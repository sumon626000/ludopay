# Database Backup — `webplustechludo`

Fresh export from MongoDB Atlas for Webuzo production import.

## Contents

- `webplustechludo/` — BSON + metadata from `mongodump`
- Collections: admins, bids, shopcoins, countries (246), userdatas, websettings, etc.

## Import on Webuzo (production)

Production database name: **`monsterg_monster`**

From project root on Webuzo SSH:

```bash
bash webuzo/import-database.sh
```

Or manually:

```bash
MONGO_URI='mongodb://monsterg_monster:YOUR_PASS@127.0.0.1:27017/monsterg_monster?authSource=admin'
mongorestore --drop --uri="$MONGO_URI" --db monsterg_monster database-backup/webplustechludo/
```

## Re-export from Atlas (local PC)

```bat
docker exec ludo-mongo mongodump --uri="YOUR_ATLAS_URI/webplustechludo" --db=webplustechludo --out=/tmp/db-export
docker cp ludo-mongo:/tmp/db-export/webplustechludo database-backup/webplustechludo
```
