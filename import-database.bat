@echo off
title Import webplustechludo Database Backup
cd /d "%~dp0"

set "ZIP=c:\Users\sumon\Downloads\All_Database_Backup\webplustechludo.zip"
set "IMPORT_DIR=%~dp0ludopay-docker\backup-import"

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker Desktop is not running.
    pause
    exit /b 1
)

if not exist "%ZIP%" (
    echo [ERROR] Backup not found:
    echo %ZIP%
    pause
    exit /b 1
)

echo Extracting backup...
if exist "%IMPORT_DIR%" rmdir /s /q "%IMPORT_DIR%"
mkdir "%IMPORT_DIR%"
powershell -NoProfile -Command "Expand-Archive -Path '%ZIP%' -DestinationPath '%IMPORT_DIR%' -Force"

echo Copying to MongoDB container...
docker cp "%IMPORT_DIR%\webplustechludo" ludo-mongo:/tmp/webplustechludo-restore

echo Importing database (this replaces existing data)...
docker exec ludo-mongo mongorestore --drop --db webplustechludo /tmp/webplustechludo-restore

echo Resetting admin password...
docker exec ludo-admin php scripts/seed-local.php

echo Restarting admin + game server...
cd /d "%~dp0ludopay-docker"
docker compose up -d --force-recreate admin
docker compose restart server

echo.
echo Collections:
docker exec ludo-mongo mongosh webplustechludo --quiet --eval "db.getCollectionNames().forEach(function(c){print(c+': '+db[c].countDocuments())})"

echo.
echo ============================================
echo   IMPORT DONE
echo   Admin: http://127.0.0.1:8000/admin
echo   Login: admin@gmail.com / NixSumon@Ludo2026
echo ============================================
pause
