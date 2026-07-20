@echo off
title Export Database Backup (zip + git update)
cd /d "%~dp0"

set "BACKUP_DIR=%~dp0database-backup"
set "DUMP_DIR=%BACKUP_DIR%\webplustechludo"
set "ZIP=%BACKUP_DIR%\webplustechludo.zip"

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker Desktop is not running.
    pause
    exit /b 1
)

echo Exporting from local Docker MongoDB...
if exist "%DUMP_DIR%" rmdir /s /q "%DUMP_DIR%"
mkdir "%DUMP_DIR%"

docker exec ludo-mongo mongodump --db=webplustechludo --out=/tmp/db-backup-export
docker cp ludo-mongo:/tmp/db-backup-export/webplustechludo/. "%DUMP_DIR%"

echo Creating zip...
if exist "%ZIP%" del /f /q "%ZIP%"
powershell -NoProfile -Command "Compress-Archive -Path '%DUMP_DIR%' -DestinationPath '%ZIP%' -Force"

echo Updating git...
git add database-backup/
git -c user.name="Sumon" -c user.email="sumon626000@gmail.com" commit -m "Update MongoDB backup zip and BSON dump."

echo.
echo Done:
echo   %ZIP%
echo   %DUMP_DIR%
dir /b "%DUMP_DIR%\*.bson"
pause
