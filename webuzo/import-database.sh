#!/bin/bash
set -e

# Webuzo MongoDB import — restores git backup into monsterg_monster
# Run from repo root: bash webuzo/import-database.sh

MONGO_USER='monsterg_monster'
MONGO_PASS='bZkq$I4%f1'
MONGO_DB='monsterg_monster'
MONGO_URI="mongodb://${MONGO_USER}:${MONGO_PASS}@127.0.0.1:27017/${MONGO_DB}?authSource=admin"

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
DUMP_DIR="$PROJECT_ROOT/database-backup/webplustechludo"

if [ ! -f "$DUMP_DIR/admins.bson" ] && [ -d "$DUMP_DIR/webplustechludo" ]; then
  DUMP_DIR="$DUMP_DIR/webplustechludo"
fi

echo "========== MongoDB import =========="
echo "Database: $MONGO_DB"
echo "Dump:     $DUMP_DIR"

if [ ! -d "$DUMP_DIR" ] || [ ! -f "$DUMP_DIR/admins.bson" ]; then
  echo "[ERROR] Backup not found. Expected admins.bson in database-backup/webplustechludo/"
  exit 1
fi

mongosh "$MONGO_URI" --eval 'db.runCommand({ ping: 1 })'
mongorestore --drop --uri="$MONGO_URI" --db="$MONGO_DB" "$DUMP_DIR"

ADMIN_DIR="${ADMIN_DIR:-/home/monsterg/admin.monstergame.app}"
if [ -f "$ADMIN_DIR/scripts/seed-local.php" ]; then
  cd "$ADMIN_DIR"
  php scripts/seed-local.php
fi

echo "Collections:"
mongosh "$MONGO_URI" --quiet --eval "db.getCollectionNames().forEach(function(c){print(c+': '+db[c].countDocuments())})"
echo "========== DONE =========="
