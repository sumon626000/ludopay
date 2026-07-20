@echo off
title Monster Game - Docker Stop
cd /d "%~dp0ludopay-docker"
echo Stopping all Docker containers...
docker compose down
echo Done.
timeout /t 3
