@echo off
echo This will DELETE all MongoDB data and re-restore from .\backup
set /p confirm=Type YES to continue: 
if /I not "%confirm%"=="YES" exit /b 0
docker compose down -v
docker compose up -d
echo Done.
pause
