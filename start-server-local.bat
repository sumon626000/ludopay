@echo off
echo ============================================
echo   LudoPay Node Server (local, no Docker)
echo ============================================
cd /d "%~dp0ludopay-docker\LudoPayServer"
set MONGO_URL=mongodb://127.0.0.1:27017/webplustechludo
set PORT=3000
echo Game server: http://localhost:3000
echo.
node ./bin/www
