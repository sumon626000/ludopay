#!/bin/sh
set -e

cd /var/www

MONGO_URI="${MONGO_DB_URI:-mongodb://mongo:27017/webplustechludo}"
MONGO_DB="${MONGO_DB_DATABASE:-webplustechludo}"

cat > .env <<EOF
APP_NAME="Monster Game"
APP_ENV=local
APP_KEY=${APP_KEY:-base64:70VyFnHQiB4ltuv3KQs2yNJ1y9sCzgLGa9Gjy9+if8A=}
APP_DEBUG=true
APP_URL=http://127.0.0.1:8000
LOG_CHANNEL=stack
LOG_LEVEL=debug
DB_CONNECTION=mongodb
MONGO_DB_URI=${MONGO_URI}
MONGO_DB_DATABASE=${MONGO_DB}
BROADCAST_DRIVER=log
CACHE_DRIVER=file
FILESYSTEM_DRIVER=local
QUEUE_CONNECTION=sync
SESSION_DRIVER=file
SESSION_LIFETIME=120
EOF

if [ ! -d vendor ]; then
  echo "Installing Composer dependencies..."
  composer install --no-interaction --prefer-dist --no-dev
fi

php artisan config:clear 2>/dev/null || true
php artisan cache:clear 2>/dev/null || true
php artisan view:clear 2>/dev/null || true

echo "Waiting for MongoDB..."
until php -r '
  $uri = getenv("MONGO_DB_URI") ?: "mongodb://mongo:27017/webplustechludo";
  $m = new MongoDB\Driver\Manager($uri);
  $m->executeCommand("webplustechludo", new MongoDB\Driver\Command(["ping" => 1]));
  echo "MongoDB OK\n";
' 2>/dev/null; do
  sleep 2
done

php scripts/seed-local.php

echo "Starting Laravel admin on http://127.0.0.1:8000/admin"
exec php artisan serve --host=0.0.0.0 --port=8000 --no-reload
