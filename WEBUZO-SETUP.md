# Webuzo Full Setup — Monster Game (All-in-One)

**Admin:** `https://admin.monstergame.app`  
**Server folder:** `/home/monsterg/public_html/server`  
**Admin folder:** `/home/monsterg/admin.monstergame.app`

---

## Credentials

| Item | Value |
|------|--------|
| MongoDB User | `monsterg_monster` |
| MongoDB Pass | `aK8l4KAxkg` |
| MongoDB DB | `monsterg_monster` |
| MongoDB URI | `mongodb://monsterg_monster:aK8l4KAxkg@127.0.0.1:27017/monsterg_monster?authSource=admin` |
| Admin Email | `admin@gmail.com` |
| Admin Password | `NixSumon@Ludo2026` |

---

## Webuzo UI (do once)

1. **Manage Database → MongoDB → Add Database** → `monsterg_monster`
2. **Manage MongoDB Users → Add User**
   - Username: `monsterg_monster`
   - Password: `aK8l4KAxkg`
   - Database: `monsterg_monster`
   - Role: **dbOwner**
3. **MongoDB Restart**
4. **Domain** `admin.monstergame.app` → document root = Laravel `public` folder
5. **SSL** ON for admin domain

---

## ALL-IN-ONE TERMINAL SCRIPT

Copy everything below and paste into Webuzo SSH terminal:

```bash
#!/bin/bash
set -e

# ============ PATHS ============
ADMIN_DIR="/home/monsterg/admin.monstergame.app"
SERVER_DIR="/home/monsterg/public_html/server"

MONGO_USER="monsterg_monster"
MONGO_PASS="aK8l4KAxkg"
MONGO_DB="monsterg_monster"
MONGO_URI="mongodb://${MONGO_USER}:${MONGO_PASS}@127.0.0.1:27017/${MONGO_DB}?authSource=admin"
ADMIN_EMAIL="admin@gmail.com"
ADMIN_PASS="NixSumon@Ludo2026"

echo "========== 1/6 MongoDB auth test =========="
mongosh "$MONGO_URI" --eval 'db.runCommand({ ping: 1 })' && echo "MongoDB OK"

echo "========== 2/6 Laravel .env =========="
cd "$ADMIN_DIR"
cp .env .env.backup.$(date +%Y%m%d_%H%M%S) 2>/dev/null || true

grep -q '^APP_ENV=' .env && sed -i 's|^APP_ENV=.*|APP_ENV=production|' .env || echo 'APP_ENV=production' >> .env
grep -q '^APP_DEBUG=' .env && sed -i 's|^APP_DEBUG=.*|APP_DEBUG=false|' .env || echo 'APP_DEBUG=false' >> .env
grep -q '^APP_URL=' .env && sed -i 's|^APP_URL=.*|APP_URL=https://admin.monstergame.app|' .env || echo 'APP_URL=https://admin.monstergame.app' >> .env
grep -q '^DB_CONNECTION=' .env && sed -i 's|^DB_CONNECTION=.*|DB_CONNECTION=mongodb|' .env || echo 'DB_CONNECTION=mongodb' >> .env
grep -q '^MONGO_DB_URI=' .env && sed -i "s|^MONGO_DB_URI=.*|MONGO_DB_URI=${MONGO_URI}|" .env || echo "MONGO_DB_URI=${MONGO_URI}" >> .env
grep -q '^MONGO_DB_DATABASE=' .env && sed -i "s|^MONGO_DB_DATABASE=.*|MONGO_DB_DATABASE=${MONGO_DB}|" .env || echo "MONGO_DB_DATABASE=${MONGO_DB}" >> .env
sed -i '/webplustechludo/d' .env
sed -i '/mongodb+srv:\/\//d' .env

echo "========== 3/6 Laravel cache + admin user =========="
php artisan config:clear
php artisan cache:clear
php artisan route:clear
php artisan view:clear
php artisan config:cache
php artisan route:cache

HASH=$(php -r "echo password_hash('${ADMIN_PASS}', PASSWORD_BCRYPT);")
mongosh "$MONGO_URI" --eval "
var existing = db.admins.findOne({ email: '${ADMIN_EMAIL}' });
if (existing) {
  db.admins.updateOne({ email: '${ADMIN_EMAIL}' }, { \$set: { password: '${HASH}' } });
  print('Admin password updated');
} else {
  db.admins.insertOne({
    name: 'Admin',
    username: 'admin',
    email: '${ADMIN_EMAIL}',
    role: 'admin',
    password: '${HASH}'
  });
  print('Admin user created');
}
"

php artisan tinker --execute="
\$a = App\Models\Admin\Admin::where('email','${ADMIN_EMAIL}')->first();
echo \$a ? (Illuminate\Support\Facades\Hash::check('${ADMIN_PASS}', \$a->password) ? 'ADMIN PASSWORD OK' : 'ADMIN PASSWORD WRONG') : 'ADMIN NOT FOUND';
"

echo ""
echo "========== 4/6 Game server Mongo fix =========="
cd "$SERVER_DIR"
sed -i "s/client.db('webplustechludo')/client.db('${MONGO_DB}')/" database/mongodatabase.js
sed -i "s/client.db('monsterg_webplustechludo')/client.db('${MONGO_DB}')/" database/mongodatabase.js
sed -i "s|mongodb://localhost:27017/webplustechludo|${MONGO_URI}|" database/mongodatabase.js

echo "========== 5/6 npm install =========="
npm install --production

echo "========== 6/6 Start game server (PM2 or nohup) =========="
export MONGO_URL="$MONGO_URI"
export PORT=3000
export NODE_ENV=production

if command -v pm2 >/dev/null 2>&1; then
  pm2 delete ludo-game-server 2>/dev/null || true
  pm2 start ./bin/www --name ludo-game-server --update-env
  pm2 save
  pm2 status
  pm2 logs ludo-game-server --lines 10 --nostream
else
  echo "PM2 not found — installing..."
  npm install -g pm2 2>/dev/null || true
  if command -v pm2 >/dev/null 2>&1; then
    pm2 start ./bin/www --name ludo-game-server --update-env
    pm2 save
    pm2 status
  else
    echo "Using nohup fallback..."
    pkill -f "node ./bin/www" 2>/dev/null || true
    nohup node ./bin/www > "$SERVER_DIR/server.log" 2>&1 &
    sleep 2
    tail -15 "$SERVER_DIR/server.log"
  fi
fi

echo ""
echo "========== DONE =========="
echo "Admin:  https://admin.monstergame.app/admin"
echo "Test:   https://admin.monstergame.app/test-db"
echo "Login:  ${ADMIN_EMAIL} / ${ADMIN_PASS}"
echo "Socket: wss://YOUR_DOMAIN:3000/socket.io/?EIO=3&transport=websocket"
```

---

## Quick one-liner (no script file)

```bash
bash -c 'ADMIN_DIR="/home/monsterg/admin.monstergame.app"; SERVER_DIR="/home/monsterg/public_html/server"; MONGO_URI="mongodb://monsterg_monster:aK8l4KAxkg@127.0.0.1:27017/monsterg_monster?authSource=admin"; cd "$ADMIN_DIR" && grep -q "^MONGO_DB_URI=" .env && sed -i "s|^MONGO_DB_URI=.*|MONGO_DB_URI=${MONGO_URI}|" .env || echo "MONGO_DB_URI=${MONGO_URI}" >> .env && grep -q "^MONGO_DB_DATABASE=" .env && sed -i "s|^MONGO_DB_DATABASE=.*|MONGO_DB_DATABASE=monsterg_monster|" .env || echo "MONGO_DB_DATABASE=monsterg_monster" >> .env && sed -i "/webplustechludo/d" .env && sed -i "/mongodb+srv:\/\//d" .env && php artisan config:clear && php artisan cache:clear && php artisan config:cache && cd "$SERVER_DIR" && sed -i "s/client.db('\''webplustechludo'\'')/client.db('\''monsterg_monster'\'')/" database/mongodatabase.js && npm install --production && MONGO_URL="$MONGO_URI" PORT=3000 NODE_ENV=production nohup node ./bin/www > server.log 2>&1 & sleep 2 && tail -10 server.log && echo DONE'
```

---

## Verify

```bash
# MongoDB
mongosh "mongodb://monsterg_monster:aK8l4KAxkg@127.0.0.1:27017/monsterg_monster?authSource=admin" --eval "db.admins.countDocuments()"

# Admin login DB check
cd /home/monsterg/admin.monstergame.app
php artisan tinker --execute="echo App\Models\Admin\Admin::where('email','admin@gmail.com')->count();"

# Game server running?
curl -s http://127.0.0.1:3000 | head -c 200
pm2 status 2>/dev/null || ps aux | grep "node ./bin/www"
tail -20 /home/monsterg/public_html/server/server.log
```

**Browser:**
- https://admin.monstergame.app/admin → login
- https://admin.monstergame.app/test-db → `"status":"connected"`

---

## Unity socket URL

| Build | URL |
|-------|-----|
| Local test | `ws://127.0.0.1:3000/socket.io/?EIO=3&transport=websocket` |
| Production | `wss://game.monstergame.app/socket.io/?EIO=3&transport=websocket` |

Set subdomain `game.monstergame.app` → reverse proxy → `127.0.0.1:3000` for Play Store WSS.

---

## Troubleshooting

| Error | Fix |
|-------|-----|
| `not authorized on webplustechludo` | Run script again; check `grep MONGO .env` shows `monsterg_monster` |
| `pm2: command not found` | Script uses nohup fallback, or `npm install -g pm2` |
| `cd: server: No such file` | Upload `LudoPayServer` to `/home/monsterg/public_html/server` |
| Admin login 500 | `php artisan config:clear && php artisan config:cache` |
| Guest login fail | Game server must show `Connected to the DB.` in logs |

---

## File locations

| What | Path |
|------|------|
| Laravel admin | `/home/monsterg/admin.monstergame.app` |
| Game server | `/home/monsterg/public_html/server` |
| Server log (nohup) | `/home/monsterg/public_html/server/server.log` |
| Local project server source | `ludopay-docker/LudoPayServer/` |
