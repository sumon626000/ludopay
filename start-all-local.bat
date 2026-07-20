@echo off
title LudoPay - Start All (Local)
cd /d "%~dp0"

echo ============================================
echo   LudoPay - Game Server + Admin (Local)
echo ============================================
echo.

where node >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Node.js not found. Install from https://nodejs.org
    pause
    exit /b 1
)

where php >nul 2>nul
if errorlevel 1 (
    echo [ERROR] PHP not found. Install XAMPP or PHP 8.x
    pause
    exit /b 1
)

if not exist "%~dp0ludopay-docker\LudoPayServer\bin\www" (
    echo [ERROR] LudoPayServer folder missing.
    pause
    exit /b 1
)

if not exist "%~dp0laravel-admin\artisan" (
    echo [ERROR] laravel-admin folder missing.
    pause
    exit /b 1
)

echo [1/2] Starting Game Server (port 3000)...
start "LudoPay Game Server" cmd /k "%~dp0start-server-local.bat"

echo [2/2] Starting Laravel Admin (port 8000)...
start "LudoPay Admin" cmd /k "%~dp0start-admin.bat"

echo.
echo Waiting for servers to start...
timeout /t 6 /nobreak >nul

echo Opening browser tabs...
start "" "http://localhost:3000"
start "" "http://127.0.0.1:8000/admin"

echo.
echo ============================================
echo   Running! Keep both CMD windows OPEN.
echo.
echo   Game Server : http://localhost:3000
echo   Admin Panel   : http://127.0.0.1:8000/admin
echo   Login         : admin@gmail.com
echo ============================================
echo.
echo To auto-start when PC turns ON, run once:
echo   install-startup.bat
echo.
pause
