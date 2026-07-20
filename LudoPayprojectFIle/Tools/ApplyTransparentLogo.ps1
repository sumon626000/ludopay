Add-Type -AssemblyName System.Drawing

function Test-BackgroundPixel([System.Drawing.Color]$c) {
    $max = [Math]::Max($c.R, [Math]::Max($c.G, $c.B))
    $min = [Math]::Min($c.R, [Math]::Min($c.G, $c.B))
    if ($max -lt 42) { return $true }
    if (($max - $min) -lt 20 -and $max -lt 58) { return $true }
    return $false
}

function Remove-LogoBackground([string]$inputPath, [string]$outputPath) {
    $src = [System.Drawing.Bitmap]::FromFile($inputPath)
    $bmp = New-Object System.Drawing.Bitmap $src.Width, $src.Height, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)

    for ($y = 0; $y -lt $src.Height; $y++) {
        for ($x = 0; $x -lt $src.Width; $x++) {
            $bmp.SetPixel($x, $y, $src.GetPixel($x, $y))
        }
    }
    $src.Dispose()

    $visited = New-Object 'System.Boolean[,]' $bmp.Width, $bmp.Height
    $queue = New-Object System.Collections.Generic.Queue[object]

    for ($x = 0; $x -lt $bmp.Width; $x++) {
        $queue.Enqueue(@($x, 0)); $queue.Enqueue(@($x, ($bmp.Height - 1)))
    }
    for ($y = 0; $y -lt $bmp.Height; $y++) {
        $queue.Enqueue(@(0, $y)); $queue.Enqueue(@(($bmp.Width - 1), $y))
    }

    while ($queue.Count -gt 0) {
        $p = $queue.Dequeue()
        $x = [int]$p[0]; $y = [int]$p[1]
        if ($x -lt 0 -or $y -lt 0 -or $x -ge $bmp.Width -or $y -ge $bmp.Height) { continue }
        if ($visited[$x, $y]) { continue }
        $visited[$x, $y] = $true

        $c = $bmp.GetPixel($x, $y)
        if (-not (Test-BackgroundPixel $c)) { continue }

        $bmp.SetPixel($x, $y, [System.Drawing.Color]::FromArgb(0, 0, 0, 0))

        $queue.Enqueue(@(($x + 1), $y))
        $queue.Enqueue(@(($x - 1), $y))
        $queue.Enqueue(@($x, ($y + 1)))
        $queue.Enqueue(@($x, ($y - 1)))
    }

    $dir = Split-Path $outputPath -Parent
    if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $bmp.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
}

function New-FittedBitmap([System.Drawing.Image]$source, [int]$width, [int]$height, [double]$fill = 0.96) {
    $bmp = New-Object System.Drawing.Bitmap $width, $height, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.Clear([System.Drawing.Color]::FromArgb(0, 0, 0, 0))
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $targetW = [int]($width * $fill)
    $targetH = [int]($height * $fill)
    $scale = [Math]::Min($targetW / $source.Width, $targetH / $source.Height)
    $nw = [int]($source.Width * $scale)
    $nh = [int]($source.Height * $scale)
    $g.DrawImage($source, [int](($width - $nw) / 2), [int](($height - $nh) / 2), $nw, $nh)
    $g.Dispose()
    return $bmp
}

function Patch-AtlasSlot([string]$atlasPath, [int]$x, [int]$y, [int]$w, [int]$h, [string]$logoPath) {
    if ((Get-Item $atlasPath).IsReadOnly) { (Get-Item $atlasPath).IsReadOnly = $false }

    $logo = [System.Drawing.Image]::FromFile($logoPath)
    $fs = [System.IO.File]::Open($atlasPath, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
    $atlas = [System.Drawing.Image]::FromStream($fs)
    $fs.Close()

    $bmp = New-Object System.Drawing.Bitmap $atlas
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy

    $clear = New-Object System.Drawing.Bitmap $w, $h, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $gc = [System.Drawing.Graphics]::FromImage($clear)
    $gc.Clear([System.Drawing.Color]::FromArgb(0, 0, 0, 0))
    $gc.Dispose()
    $g.DrawImage($clear, $x, $y)
    $clear.Dispose()

    $g.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceOver
    $slot = New-FittedBitmap $logo $w $h
    $g.DrawImage($slot, $x, $y)
    $slot.Dispose()
    $g.Dispose()

    $tmp = "$atlasPath.tmp.png"
    $bmp.Save($tmp, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose(); $atlas.Dispose(); $logo.Dispose()
    Move-Item $tmp $atlasPath -Force
}

$project = Split-Path $PSScriptRoot -Parent
$raw = "$project\Assets\Textures\Logo\MonsterGameLogo_Source.png"
$transparent = "$project\Assets\Textures\Logo\MonsterGameLogo_Transparent.png"

Remove-LogoBackground $raw $transparent
Write-Output "Transparent logo created"

$targets = @(
    "$project\Assets\LudoPaySprites\ludo pay logo-02.png",
    "$project\Assets\LudoPaySprites\ludo pay logo-01.png",
    "$project\Assets\LudoPaySprites\CompanyLogo.png",
    "$project\Assets\Textures\Logo\MonsterGameLogo_512.png",
    "$project\Assets\Textures\Logo\MonsterGameLogo_256.png",
    "$project\Assets\EpicVictoryEffects\Textures\Logo_SplashScreen_01.png"
)
foreach ($t in $targets) { Copy-Item $transparent $t -Force }

Patch-AtlasSlot "$project\Assets\Atlases\LudoPay.png" 1652 199 408 241 $transparent
Patch-AtlasSlot "$project\Assets\Atlases\LudoGamePlay1.png" 3045 3900 586 196 $transparent
Write-Output "All logos updated with transparent background"
