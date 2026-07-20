@echo off
title Switch to Local Docker MongoDB
cd /d "%~dp0ludopay-docker"

echo Switching Docker to local MongoDB...
(
echo ADMIN_USERNAME=admin
echo ADMIN_PASSWORD=NixSumon@Ludo2026
echo JWT_SECRET=my-super-secret-random-string-12345
echo.
echo MONGO_DB_URI=mongodb://mongo:27017/webplustechludo
echo MONGO_DB_DATABASE=webplustechludo
) > .env

docker compose up -d mongo
docker compose up -d --force-recreate admin server
timeout /t 20 /nobreak >nul
docker exec ludo-admin php artisan config:clear
docker exec ludo-admin php scripts/seed-local.php
docker exec ludo-admin php scripts/verify-admin-login.php

echo.
echo ============================================
echo   Now using local Docker MongoDB
echo   Admin: http://127.0.0.1:8000/admin
echo ============================================
pause
