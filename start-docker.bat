@echo off
title Monster Game - Docker Start
cd /d "%~dp0ludopay-docker"

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker Desktop not installed.
    echo Download: https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

echo ============================================
echo   Monster Game - DOCKER (Mongo + Server + Admin)
echo ============================================
echo.

if not exist .env copy .env.example .env >nul

echo Building and starting containers...
docker compose up -d --build

echo.
echo Waiting for services...
timeout /t 12 /nobreak >nul

echo.
echo ============================================
echo   RUNNING (Docker)
echo.
echo   MongoDB      : localhost:27017
echo   Game Server  : http://localhost:3000
echo   Admin Panel  : http://127.0.0.1:8000/admin
echo   Login        : admin@gmail.com / NixSumon@Ludo2026
echo.
echo   Unity Socket : ws://127.0.0.1:3000/socket.io/?EIO=3^&transport=websocket
echo.
echo   Logs   : ludopay-docker\logs.bat
echo   Stop   : stop-docker.bat
echo ============================================
echo.

start "" "http://127.0.0.1:8000/admin"
start "" "http://localhost:3000"

pause
