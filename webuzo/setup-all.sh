#!/bin/bash
set -e

# ============================================================
# Monster Game — Webuzo ALL-IN-ONE SETUP
# Run on Webuzo SSH after: git clone YOUR_REPO ludopay-repo
# ============================================================

MONGO_USER='monsterg_monster'
MONGO_PASS='bZkq$I4%f1'
MONGO_DB='monsterg_monster'
MONGO_URI="mongodb://${MONGO_USER}:${MONGO_PASS}@127.0.0.1:27017/${MONGO_DB}?authSource=admin"

REPO_DIR="${REPO_DIR:-/home/monsterg/ludopay-repo}"
ADMIN_DIR="${ADMIN_DIR:-/home/monsterg/admin.monstergame.app}"
SERVER_DIR="${SERVER_DIR:-/home/monsterg/public_html/server}"
ADMIN_EMAIL='admin@gmail.com'
ADMIN_PASS='NixSumon@Ludo2026'

echo "========== 1/7 Copy code from git =========="
mkdir -p "$ADMIN_DIR" "$SERVER_DIR"
rsync -a "$REPO_DIR/laravel-admin/" "$ADMIN_DIR/" --exclude vendor --exclude node_modules --exclude storage/logs
rsync -a "$REPO_DIR/ludopay-docker/LudoPayServer/" "$SERVER_DIR/" --exclude node_modules

echo "========== 2/7 MongoDB test =========="
mongosh "$MONGO_URI" --eval 'db.runCommand({ ping: 1 })'

echo "========== 3/7 Import database from git backup =========="
DUMP_DIR="$REPO_DIR/database-backup/webplustechludo"
if [ ! -f "$DUMP_DIR/admins.bson" ] && [ -d "$DUMP_DIR/webplustechludo" ]; then
  DUMP_DIR="$DUMP_DIR/webplustechludo"
fi
mongorestore --drop --uri="$MONGO_URI" --db="$MONGO_DB" "$DUMP_DIR"

echo "========== 4/7 Laravel .env + composer =========="
cd "$ADMIN_DIR"
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
php artisan route:clear
php artisan view:clear
php artisan config:cache
php artisan route:cache
chmod -R 775 storage bootstrap/cache
php scripts/seed-local.php

echo "========== 5/7 Game server =========="
cd "$SERVER_DIR"
sed -i "s/client.db('webplustechludo')/client.db('monsterg_monster')/" database/mongodatabase.js
sed -i "s/client.db('monsterg_webplustechludo')/client.db('monsterg_monster')/" database/mongodatabase.js
npm install --production

echo "========== 6/7 Start game server (PM2) =========="
export MONGO_URL="$MONGO_URI"
export PORT=3000
export NODE_ENV=production
pm2 delete ludo-server 2>/dev/null || true
pm2 start ./bin/www --name ludo-server
pm2 save

echo "========== 7/7 Verify =========="
mongosh "$MONGO_URI" --quiet --eval 'print("admins: "+db.admins.countDocuments())'
curl -s -o /dev/null -w "Admin HTTP: %{http_code}\n" http://127.0.0.1:8000/admin 2>/dev/null || true
curl -s -o /dev/null -w "Server HTTP: %{http_code}\n" http://127.0.0.1:3000/ || true

echo ""
echo "============================================"
echo "  SETUP COMPLETE"
echo "  Admin:  https://admin.monstergame.app/admin"
echo "  Login:  $ADMIN_EMAIL / $ADMIN_PASS"
echo "  Socket: wss://monstergame.app/socket.io/?EIO=3&transport=websocket"
echo "  (enable /socket.io reverse proxy on main domain)"
echo "============================================"
