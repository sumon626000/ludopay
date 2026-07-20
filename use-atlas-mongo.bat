@echo off
title Switch to MongoDB Atlas
cd /d "%~dp0ludopay-docker"

if not exist .env (
  echo [ERROR] ludopay-docker\.env not found. Copy .env.example and set MONGO_DB_URI.
  pause
  exit /b 1
)

echo Switching Docker to MongoDB Atlas...
docker compose up -d --force-recreate admin server
timeout /t 20 /nobreak >nul
docker exec ludo-admin php artisan config:clear
docker exec ludo-admin php scripts/verify-admin-login.php

echo.
echo ============================================
echo   Now using MongoDB Atlas (from ludopay-docker\.env)
echo   Admin: http://127.0.0.1:8000/admin
echo ============================================
pause
