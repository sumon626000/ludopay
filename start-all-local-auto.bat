@echo off
cd /d "%~dp0"

where node >nul 2>nul
if errorlevel 1 exit /b 1
where php >nul 2>nul
if errorlevel 1 exit /b 1

start "LudoPay Game Server" /min cmd /k "%~dp0start-server-local.bat"
start "LudoPay Admin" /min cmd /k "%~dp0start-admin.bat"

timeout /t 8 /nobreak >nul
start "" "http://localhost:3000"
start "" "http://127.0.0.1:8000/admin"
exit /b 0
