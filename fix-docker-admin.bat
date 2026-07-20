@echo off
title Fix Docker Admin Login
cd /d "%~dp0ludopay-docker"

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker Desktop is not running.
    pause
    exit /b 1
)

echo Fixing admin MongoDB connection (force-recreate so .env is rewritten)...
docker compose up -d mongo server
docker compose up -d --force-recreate admin

echo Waiting for admin container...
timeout /t 20 /nobreak >nul

docker exec ludo-admin php artisan config:clear
docker exec ludo-admin php scripts/seed-local.php
docker exec ludo-admin php scripts/verify-admin-login.php

echo.
echo MongoDB URI in container:
docker exec ludo-admin findstr MONGO_DB_URI .env

echo.
echo ============================================
echo   Try login now:
echo   http://127.0.0.1:8000/admin
echo   admin@gmail.com / NixSumon@Ludo2026
echo ============================================
pause
