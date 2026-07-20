# Webuzo Deploy from Git

Deploy Monster Game admin + game server + database on Webuzo using this repo.

## Server paths (Webuzo)

| Service | Path |
|---------|------|
| Laravel admin | `/home/monsterg/admin.monstergame.app` |
| Game server | `/home/monsterg/public_html/server` |
| Admin URL | `https://admin.monstergame.app/admin` |

---

## Step 1 — Git clone on Webuzo

SSH into Webuzo, then:

```bash
cd /home/monsterg

# Clone repo (replace with your GitHub URL)
git clone https://github.com/YOUR_USER/ludopay.git ludopay-repo

# Copy admin files
rsync -a --delete ludopay-repo/laravel-admin/ admin.monstergame.app/ \
  --exclude vendor --exclude node_modules --exclude storage/logs

# Copy game server
rsync -a --delete ludopay-repo/ludopay-docker/LudoPayServer/ public_html/server/ \
  --exclude node_modules
```

---

## Step 2 — MongoDB (Webuzo UI)

1. **Manage Database → MongoDB → Add Database** → `monsterg_monster`
2. **Add User** → `monsterg_monster` / your password / role **dbOwner**
3. **Restart MongoDB**

---

## Step 3 — Import database from git

```bash
cd /home/monsterg/ludopay-repo
chmod +x webuzo/import-database.sh
bash webuzo/import-database.sh
```

This restores `database-backup/webplustechludo/` into **`monsterg_monster`**.

---

## Step 4 — Laravel admin `.env`

```bash
cd /home/monsterg/admin.monstergame.app

cat > .env <<'EOF'
APP_NAME="Monster Game"
APP_ENV=production
APP_KEY=base64:70VyFnHQiB4ltuv3KQs2yNJ1y9sCzgLGa9Gjy9+if8A=
APP_DEBUG=false
APP_URL=https://admin.monstergame.app
DB_CONNECTION=mongodb
MONGO_DB_URI=mongodb://monsterg_monster:YOUR_MONGO_PASS@127.0.0.1:27017/monsterg_monster?authSource=admin
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
```

Domain document root must point to Laravel **`public`** folder.

---

## Step 5 — Game server

```bash
cd /home/monsterg/public_html/server

# Fix DB name in mongodatabase.js if needed
sed -i "s/client.db('webplustechludo')/client.db('monsterg_monster')/" database/mongodatabase.js

npm install --production

export MONGO_URL='mongodb://monsterg_monster:YOUR_MONGO_PASS@127.0.0.1:27017/monsterg_monster?authSource=admin'
export PORT=3000
export NODE_ENV=production

# Start with PM2 (recommended)
pm2 delete ludo-server 2>/dev/null || true
pm2 start ./bin/www --name ludo-server
pm2 save
```

Or without PM2:

```bash
nohup node ./bin/www > server.log 2>&1 &
```

---

## Step 6 — Verify

```bash
mongosh 'mongodb://monsterg_monster:YOUR_MONGO_PASS@127.0.0.1:27017/monsterg_monster?authSource=admin' \
  --eval 'db.admins.countDocuments()'

curl -I https://admin.monstergame.app/admin
curl http://127.0.0.1:3000/
```

**Admin login:** `admin@gmail.com` / `NixSumon@Ludo2026`

---

## Update later (git pull)

```bash
cd /home/monsterg/ludopay-repo
git pull

rsync -a ludopay-repo/laravel-admin/ /home/monsterg/admin.monstergame.app/ --exclude vendor --exclude .env
rsync -a ludopay-repo/ludopay-docker/LudoPayServer/ /home/monsterg/public_html/server/ --exclude node_modules

cd /home/monsterg/admin.monstergame.app && php artisan config:cache
pm2 restart ludo-server
```

Full script also in `WEBUZO-SETUP.md`.
