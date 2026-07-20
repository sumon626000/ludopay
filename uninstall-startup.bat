@echo off
title LudoPay - Remove Auto Start
set "LINK=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\LudoPay-StartAll.lnk"

if exist "%LINK%" (
    del "%LINK%"
    echo [OK] Auto-start removed.
) else (
    echo Auto-start shortcut not found.
)
pause
