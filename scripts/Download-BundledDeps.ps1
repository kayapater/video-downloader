param(
    [switch]$SkipYtDlp,
    [switch]$SkipFFmpeg
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Split-Path -Parent $scriptDir
$bundledDir = Join-Path $projectDir "bundled"

Write-Host "=== Video Downloader — Bundled Dependency Downloader ===" -ForegroundColor Cyan
Write-Host "Target: $bundledDir"
New-Item -ItemType Directory -Force -Path $bundledDir | Out-Null

# ── yt-dlp ────────────────────────────────────────────────────────
if (-not $SkipYtDlp) {
    $ytDlpPath = Join-Path $bundledDir "yt-dlp.exe"
    if (Test-Path $ytDlpPath) {
        Write-Host "✅ yt-dlp.exe already present" -ForegroundColor Green
    } else {
        Write-Host "⬇ Downloading yt-dlp.exe..." -ForegroundColor Yellow
        $url = "https://github.com/yt-dlp/yt-dlp/releases/download/2026.03.17/yt-dlp.exe"
        Invoke-WebRequest -Uri $url -OutFile $ytDlpPath -UseBasicParsing
        Write-Host "✅ yt-dlp.exe downloaded" -ForegroundColor Green
    }
}

# ── FFmpeg ────────────────────────────────────────────────────────
if (-not $SkipFFmpeg) {
    $ffmpegExe = Join-Path $bundledDir "ffmpeg.exe"
    if (Test-Path $ffmpegExe) {
        Write-Host "✅ FFmpeg already present" -ForegroundColor Green
    } else {
        Write-Host "⬇ Downloading FFmpeg (this may take a minute)..." -ForegroundColor Yellow
        $tempDir = Join-Path $bundledDir ".tmp_ffmpeg"
        $zipPath = Join-Path $tempDir "ffmpeg.zip"
        $extractPath = Join-Path $tempDir "extract"

        try {
            New-Item -ItemType Directory -Force -Path $tempDir, $extractPath | Out-Null

            $url = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip"
            Invoke-WebRequest -Uri $url -OutFile $zipPath -UseBasicParsing

            Write-Host "📦 Extracting FFmpeg..." -ForegroundColor Yellow
            Expand-Archive -LiteralPath $zipPath -DestinationPath $extractPath -Force

            $foundFfmpeg = Get-ChildItem -Path $extractPath -Recurse -Filter "ffmpeg.exe" | Select-Object -First 1
            if (-not $foundFfmpeg) {
                throw "ffmpeg.exe not found in extracted archive"
            }

            $binDir = Split-Path -Parent $foundFfmpeg.FullName
            foreach ($tool in @("ffmpeg.exe", "ffprobe.exe", "ffplay.exe")) {
                $src = Join-Path $binDir $tool
                if (Test-Path $src) {
                    Copy-Item $src $bundledDir -Force
                    Write-Host "  ✅ $tool" -ForegroundColor Green
                }
            }
        } catch {
            throw
        } finally {
            Remove-Item -Recurse -Force $tempDir -ErrorAction SilentlyContinue
        }
    }
}

Write-Host ""
Write-Host "=== Done! All dependencies are in: $bundledDir ===" -ForegroundColor Cyan
Get-ChildItem $bundledDir -Filter "*.exe" | ForEach-Object {
    $size = "{0:N1} MB" -f ($_.Length / 1MB)
    Write-Host "  📄 $($_.Name) ($size)" -ForegroundColor White
}
