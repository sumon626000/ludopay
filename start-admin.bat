@echo off
echo ============================================
echo   LudoPay Laravel Admin (local PHP)
echo ============================================
echo.

where php >nul 2>nul
if errorlevel 1 (
    echo [ERROR] PHP is not installed. Install XAMPP or PHP 8.x
    pause
    exit /b 1
)

cd /d "%~dp0laravel-admin"
if not exist artisan (
    echo [ERROR] laravel-admin folder not found.
    pause
    exit /b 1
)

echo Admin panel: http://127.0.0.1:8000/admin
echo.
php artisan serve --host=127.0.0.1 --port=8000
