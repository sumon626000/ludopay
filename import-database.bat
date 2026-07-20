@echo off
title Import webplustechludo Database Backup
cd /d "%~dp0"

set "ZIP=c:\Users\sumon\Downloads\All_Database_Backup\webplustechludo.zip"
set "IMPORT_DIR=%~dp0database-backup"
set "DUMP_DIR=%IMPORT_DIR%\webplustechludo"

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

echo Extracting backup into database-backup\webplustechludo...
if exist "%DUMP_DIR%" rmdir /s /q "%DUMP_DIR%"
mkdir "%DUMP_DIR%"
powershell -NoProfile -Command "Expand-Archive -Path '%ZIP%' -DestinationPath '%IMPORT_DIR%\_tmp' -Force; Copy-Item '%IMPORT_DIR%\_tmp\webplustechludo\*' '%DUMP_DIR%' -Recurse -Force; Remove-Item '%IMPORT_DIR%\_tmp' -Recurse -Force"

echo Creating database-backup\webplustechludo.zip...
if exist "%IMPORT_DIR%\webplustechludo.zip" del /f /q "%IMPORT_DIR%\webplustechludo.zip"
powershell -NoProfile -Command "Compress-Archive -Path '%DUMP_DIR%' -DestinationPath '%IMPORT_DIR%\webplustechludo.zip' -Force"

echo Importing into local Docker MongoDB...
docker cp "%DUMP_DIR%" ludo-mongo:/tmp/webplustechludo-restore
docker exec ludo-mongo mongorestore --drop --db webplustechludo /tmp/webplustechludo-restore

echo Resetting admin password...
docker exec ludo-admin php scripts/seed-local.php

echo Updating git...
git add database-backup/
git -c user.name="Sumon" -c user.email="sumon626000@gmail.com" commit -m "Update database backup from webplustechludo.zip" 2>nul

echo Restarting admin + game server...
cd /d "%~dp0ludopay-docker"
docker compose up -d --force-recreate admin
docker compose restart server

echo.
echo ============================================
echo   IMPORT DONE + GIT UPDATED
echo   Webuzo import: bash webuzo/import-database.sh
echo ============================================
pause
