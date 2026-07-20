@echo off
echo ============================================
echo   LudoPay - Full Setup (Docker + Admin)
echo ============================================
echo.

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker Desktop is not installed or not in PATH.
    pause
    exit /b 1
)

cd /d "%~dp0ludopay-docker"
if not exist LudoPayServer (
    echo [ERROR] LudoPayServer folder missing in ludopay-docker.
    pause
    exit /b 1
)

if not exist .env (
    copy .env.example .env >nul
)

echo [1/3] Starting MongoDB + Node server (Docker)...
docker compose up -d --build
if errorlevel 1 (
    echo [WARN] Docker not running - starting Node server locally instead...
    start "LudoPay Server" cmd /k "cd /d "%~dp0" && start-server-local.bat"
    goto :admin
)

:admin
echo [2/3] Starting Laravel admin (local PHP)...
start "LudoPay Admin" cmd /k "cd /d "%~dp0laravel-admin" && php artisan serve --host=127.0.0.1 --port=8000"

echo [3/3] Opening browser...
timeout /t 4 /nobreak >nul
start http://127.0.0.1:8000/admin
start http://localhost:3000

echo.
echo ============================================
echo   Running!
echo   Game Server : http://localhost:3000
echo   Laravel Admin: http://127.0.0.1:8000/admin
echo   MongoDB     : localhost:27017
echo ============================================
echo.
echo Stop Docker: ludopay-docker\stop.bat
echo Stop Admin: close the "LudoPay Admin" window
pause
