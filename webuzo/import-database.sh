#!/bin/bash
set -e

# Import git database-backup into Webuzo local MongoDB
# Run from project root after git clone

MONGO_USER="${MONGO_USER:-monsterg_monster}"
MONGO_PASS="${MONGO_PASS:-aK8l4KAxkg}"
MONGO_DB="${MONGO_DB:-monsterg_monster}"
MONGO_URI="mongodb://${MONGO_USER}:${MONGO_PASS}@127.0.0.1:27017/${MONGO_DB}?authSource=admin"

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
DUMP_DIR="$PROJECT_ROOT/database-backup/webplustechludo"
# Support nested export folder from zip extract
if [ ! -f "$DUMP_DIR/admins.bson" ] && [ -d "$DUMP_DIR/webplustechludo" ]; then
  DUMP_DIR="$DUMP_DIR/webplustechludo"
fi

echo "========== MongoDB import for Webuzo =========="
echo "Database: $MONGO_DB"
echo "Dump dir: $DUMP_DIR"

if [ ! -d "$DUMP_DIR" ]; then
  echo "[ERROR] Dump not found: $DUMP_DIR"
  echo "Run git pull or copy database-backup folder first."
  exit 1
fi

echo "Testing MongoDB connection..."
mongosh "$MONGO_URI" --eval 'db.runCommand({ ping: 1 })'

echo "Importing (this replaces existing data in $MONGO_DB)..."
mongorestore --drop --uri="$MONGO_URI" --db="$MONGO_DB" "$DUMP_DIR"

echo "Setting admin password..."
ADMIN_DIR="${ADMIN_DIR:-/home/monsterg/admin.monstergame.app}"
if [ -f "$ADMIN_DIR/scripts/seed-local.php" ]; then
  cd "$ADMIN_DIR"
  php scripts/seed-local.php
else
  echo "Skip seed — run manually: php scripts/seed-local.php"
fi

echo "Collections:"
mongosh "$MONGO_URI" --quiet --eval "db.getCollectionNames().forEach(function(c){print(c+': '+db[c].countDocuments())})"

echo "========== IMPORT DONE =========="
echo "Admin: https://admin.monstergame.app/admin"
echo "Login: admin@gmail.com / NixSumon@Ludo2026"
