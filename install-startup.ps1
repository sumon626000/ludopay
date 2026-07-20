$target = Join-Path $PSScriptRoot "start-all-local-auto.bat"
$linkPath = Join-Path $env:APPDATA "Microsoft\Windows\Start Menu\Programs\Startup\LudoPay-StartAll.lnk"

if (-not (Test-Path $target)) {
    Write-Host "[ERROR] start-all-local-auto.bat not found."
    exit 1
}

$ws = New-Object -ComObject WScript.Shell
$s = $ws.CreateShortcut($linkPath)
$s.TargetPath = $target
$s.WorkingDirectory = $PSScriptRoot
$s.WindowStyle = 1
$s.Description = "LudoPay Game Server + Admin"
$s.Save()

if (Test-Path $linkPath) {
    Write-Host "[OK] Auto-start installed:"
    Write-Host $linkPath
} else {
    Write-Host "[ERROR] Shortcut create failed."
    exit 1
}
