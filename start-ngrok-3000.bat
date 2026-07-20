@echo off
setlocal EnableDelayedExpansion
title LudoPay ngrok tunnel (port 3000)
cd /d "%~dp0"

set "NGROK="
where ngrok >nul 2>&1
if %ERRORLEVEL% equ 0 (
  for /f "delims=" %%I in ('where ngrok 2^>nul') do (
    set "NGROK=%%I"
    goto :found
  )
)
if exist "%LOCALAPPDATA%\Microsoft\WinGet\Packages\Ngrok.Ngrok_Microsoft.Winget.Source_8wekyb3d8bbwe\ngrok.exe" (
  set "NGROK=%LOCALAPPDATA%\Microsoft\WinGet\Packages\Ngrok.Ngrok_Microsoft.Winget.Source_8wekyb3d8bbwe\ngrok.exe"
)
:found
if not defined NGROK (
  echo [ERROR] ngrok not found. Install: winget install Ngrok.Ngrok
  echo Or download from https://ngrok.com/download
  pause
  exit /b 1
)

echo Checking LudoPay on http://127.0.0.1:3000 ...
where curl >nul 2>&1
if %ERRORLEVEL% equ 0 (
  curl -s -o nul -w "HTTP %%{http_code}\n" --connect-timeout 2 http://127.0.0.1:3000/
  if errorlevel 1 echo [WARN] Could not reach port 3000. Start LudoPay server first.
) else (
  powershell -NoProfile -Command "try { (Invoke-WebRequest -Uri 'http://127.0.0.1:3000/' -UseBasicParsing -TimeoutSec 2).StatusCode } catch { Write-Host '[WARN] Port 3000 not responding. Start LudoPay server first.' }"
)

echo.
echo Starting ngrok: public URL for localhost:3000
echo Dashboard: http://127.0.0.1:4040
echo.
"%NGROK%" http 3000
set "EC=%ERRORLEVEL%"
if %EC% neq 0 (
  echo.
  echo [HELP] First time? Sign up at https://dashboard.ngrok.com/signup
  echo Copy your authtoken from https://dashboard.ngrok.com/get-started/your-authtoken
  echo Then run once in CMD:
  echo   ngrok config add-authtoken YOUR_TOKEN_HERE
  echo Or:
  echo   "%NGROK%" config add-authtoken YOUR_TOKEN_HERE
  pause
)
exit /b %EC%
