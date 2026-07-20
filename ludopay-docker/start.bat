@echo off
echo ============================================
echo   Monster Game - Docker Start
echo ============================================
echo.

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker Desktop not installed.
    pause
    exit /b 1
)

if not exist .env copy .env.example .env >nul

echo Starting MongoDB + Game Server + Laravel Admin...
docker compose up -d --build

echo.
echo ============================================
echo   Running!
echo   MongoDB      : localhost:27017
echo   Game Server  : http://localhost:3000
echo   Admin Panel  : http://127.0.0.1:8000/admin
echo   Login        : admin@gmail.com / NixSumon@Ludo2026
echo ============================================
echo.
echo Logs: logs.bat
echo Stop: stop.bat
pause
