# Webuzo Full Setup Guide — Monster Game

**Domain:** `monstergame.app` / `admin.monstergame.app`  
**Admin login:** `admin@gmail.com` / `NixSumon@Ludo2026`

---

## Your MongoDB (Webuzo)

| Item | Value |
|------|--------|
| Database | `monsterg_monster` |
| Username | `monsterg_monster` |
| Password | `bZkq$I4%f1` |
| URI | `mongodb://monsterg_monster:bZkq$I4%f1@127.0.0.1:27017/monsterg_monster?authSource=admin` |

---

## Server folders

| Service | Path |
|---------|------|
| Git clone | `/home/monsterg/ludopay-repo` |
| Laravel admin | `/home/monsterg/admin.monstergame.app` |
| Game server | `/home/monsterg/public_html/server` |
| Admin URL | `https://admin.monstergame.app/admin` |

---

## PART A — Webuzo UI (do once)

### 1. MongoDB
1. **Webuzo → Manage Database → MongoDB**
2. Add database: **`monsterg_monster`**
3. Add user:
   - Username: **`monsterg_monster`**
   - Password: **`bZkq$I4%f1`**
   - Database: **`monsterg_monster`**
   - Role: **dbOwner**
4. **Restart MongoDB**

### 2. Admin domain
1. Add domain **`admin.monstergame.app`**
2. Document root → Laravel **`public`** folder:
   ```
   /home/monsterg/admin.monstergame.app/public
   ```
3. Enable **SSL (Let's Encrypt)**

### 3. PHP
- PHP **8.1+** or **8.2** enabled for admin domain
- Extensions: `mongodb`, `zip`, `mbstring`, `openssl`

---

## PART B — Git clone on Webuzo SSH

Replace `YOUR_GITHUB_URL` with:

```
https://github.com/sumon626000/ludopay.git
```

```bash
cd /home/monsterg
git clone https://github.com/sumon626000/ludopay.git ludopay-repo
cd ludopay-repo
chmod +x webuzo/*.sh
```

---

## PART C — ONE COMMAND setup (recommended)

```bash
cd /home/monsterg/ludopay-repo
bash webuzo/setup-all.sh
```

This script:
1. Copies Laravel admin + Node game server from git
2. Tests MongoDB connection
3. Imports **`database-backup/webplustechludo/`** → **`monsterg_monster`**
4. Creates Laravel `.env`
5. Runs `composer install` + cache
6. Resets admin password
7. Starts game server with **PM2** on port **3000**

---

## PART D — Import database only (manual)

If you only need DB import:

```bash
cd /home/monsterg/ludopay-repo
bash webuzo/import-database.sh
```

Or from zip on server:

```bash
cd /home/monsterg/ludopay-repo
unzip -o database-backup/webplustechludo.zip -d /tmp/
mongorestore --drop \
  --uri='mongodb://monsterg_monster:bZkq$I4%f1@127.0.0.1:27017/monsterg_monster?authSource=admin' \
  --db monsterg_monster /tmp/webplustechludo/
```

---

## PART E — Laravel `.env` (if not using setup-all.sh)

```bash
cd /home/monsterg/admin.monstergame.app

cat > .env <<'EOF'
APP_NAME="Monster Game"
APP_ENV=production
APP_KEY=base64:70VyFnHQiB4ltuv3KQs2yNJ1y9sCzgLGa9Gjy9+if8A=
APP_DEBUG=false
APP_URL=https://admin.monstergame.app
DB_CONNECTION=mongodb
MONGO_DB_URI=mongodb://monsterg_monster:bZkq$I4%f1@127.0.0.1:27017/monsterg_monster?authSource=admin
MONGO_DB_DATABASE=monsterg_monster
CACHE_DRIVER=file
SESSION_DRIVER=file
QUEUE_CONNECTION=sync
EOF

composer install --no-dev --optimize-autoloader
php artisan config:clear
php artisan cache:clear
php artisan config:cache
php artisan route:cache
chmod -R 775 storage bootstrap/cache
php scripts/seed-local.php
```

---

## PART F — Game server

```bash
cd /home/monsterg/public_html/server

sed -i "s/client.db('webplustechludo')/client.db('monsterg_monster')/" database/mongodatabase.js
npm install --production

export MONGO_URL='mongodb://monsterg_monster:bZkq$I4%f1@127.0.0.1:27017/monsterg_monster?authSource=admin'
export PORT=3000
export NODE_ENV=production

pm2 delete ludo-server 2>/dev/null || true
pm2 start ./bin/www --name ludo-server
pm2 save
pm2 startup
```

Without PM2:

```bash
nohup node ./bin/www > server.log 2>&1 &
```

---

## PART G — One domain socket proxy (optional)

To use **`wss://monstergame.app/socket.io/`** on same domain, add Apache proxy on main domain vhost:

```apache
ProxyPass /socket.io http://127.0.0.1:3000/socket.io
ProxyPassReverse /socket.io http://127.0.0.1:3000/socket.io
```

Unity Play Store socket URL:
```
wss://monstergame.app/socket.io/?EIO=3&transport=websocket
```

---

## PART H — Verify

```bash
# MongoDB
mongosh 'mongodb://monsterg_monster:bZkq$I4%f1@127.0.0.1:27017/monsterg_monster?authSource=admin' \
  --eval 'db.admins.countDocuments()'

# Admin panel
curl -I https://admin.monstergame.app/admin

# Game server
curl http://127.0.0.1:3000/
pm2 logs ludo-server --lines 20
```

Open browser: **https://admin.monstergame.app/admin**  
Login: **`admin@gmail.com`** / **`NixSumon@Ludo2026`**

---

## PART I — Update from git later

```bash
cd /home/monsterg/ludopay-repo
git pull

rsync -a ludopay-repo/laravel-admin/ /home/monsterg/admin.monstergame.app/ \
  --exclude vendor --exclude .env --exclude storage/logs

rsync -a ludopay-repo/ludopay-docker/LudoPayServer/ /home/monsterg/public_html/server/ \
  --exclude node_modules

cd /home/monsterg/admin.monstergame.app
php artisan config:cache

pm2 restart ludo-server
```

---

## Troubleshooting

| Error | Fix |
|-------|-----|
| `not authorized on monsterg_monster` | Check Webuzo MongoDB user/password matches above |
| Admin 500 | `php artisan config:clear && php artisan cache:clear` |
| Login 500 MongoDB | `.env` must use `127.0.0.1` not Atlas on Webuzo |
| Port 3000 busy | `fuser -k 3000/tcp` then `pm2 restart ludo-server` |
| Empty dashboard | Run `bash webuzo/import-database.sh` again |

---

## Git repo contents

| Path | Purpose |
|------|---------|
| `laravel-admin/` | Admin panel source |
| `ludopay-docker/LudoPayServer/` | Game server source |
| `database-backup/webplustechludo/` | MongoDB BSON dump |
| `database-backup/webplustechludo.zip` | Same backup as zip |
| `webuzo/setup-all.sh` | Full auto setup |
| `webuzo/import-database.sh` | DB import only |
| `LudoPayprojectFIle/` | Unity game project |

---

## Push git from Windows PC

```bat
cd c:\Users\sumon\Desktop\monster\ludopay
git remote add origin https://github.com/sumon626000/ludopay.git
git push -u origin master
```

Repo: **https://github.com/sumon626000/ludopay**
