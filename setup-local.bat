@echo off
title Monster Game - Docker Setup
cd /d "%~dp0"

where docker >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Install Docker Desktop first: https://docker.com/products/docker-desktop
    pause
    exit /b 1
)

echo Copying local .env for optional host-side PHP...
copy /Y "%~dp0laravel-admin\.env.local" "%~dp0laravel-admin\.env" >nul 2>nul

call "%~dp0start-docker.bat"
