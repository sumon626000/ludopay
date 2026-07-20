@echo off
title LudoPay - Stop All
echo Stopping LudoPay servers on port 3000 and 8000...

for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3000" ^| findstr "LISTENING"') do (
    echo Stopping Game Server PID %%a
    taskkill /PID %%a /F >nul 2>nul
)

for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":8000" ^| findstr "LISTENING"') do (
    echo Stopping Admin PID %%a
    taskkill /PID %%a /F >nul 2>nul
)

echo Done. Close any "LudoPay" CMD windows if still open.
timeout /t 3
