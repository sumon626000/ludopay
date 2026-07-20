@echo off
title Seed Default Game Database
cd /d "%~dp0ludopay-docker"

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker Desktop is not running.
    pause
    exit /b 1
)

echo Seeding webplustechludo database...
docker exec ludo-mongo mongosh webplustechludo --file /docker-entrypoint-initdb.d/02-default-game-data.js

echo.
echo Current collections:
docker exec ludo-mongo mongosh webplustechludo --quiet --eval "db.getCollectionNames().forEach(function(c){print(c+': '+db[c].countDocuments())})"

echo.
echo Done. Restart game if needed: docker compose restart server
pause
