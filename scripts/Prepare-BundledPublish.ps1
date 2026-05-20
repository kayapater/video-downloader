param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [switch]$SkipPublish
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
$projectPath = Join-Path $repoRoot "VideoDownloader/VideoDownloader.csproj"
$versionSourcePath = Join-Path $repoRoot "VideoDownloader/DependencyVersions.cs"
$publishDir = Join-Path $repoRoot "VideoDownloader/publish"
$tempDir = Join-Path $repoRoot ".tmp_bundle"

if (-not (Test-Path -LiteralPath $versionSourcePath)) {
    throw "Dependency version source not found: $versionSourcePath"
}

$versionSourceText = Get-Content -LiteralPath $versionSourcePath -Raw

function Get-ConstValue {
    param([string]$Name)

    $pattern = 'public\s+const\s+string\s+' + [Regex]::Escape($Name) + '\s*=\s*"([^"]+)";'
    $match = [Regex]::Match($versionSourceText, $pattern)
    if (-not $match.Success) {
        throw "Could not resolve $Name from $versionSourcePath"
    }

    return $match.Groups[1].Value
}

$bundledYtDlpVersion = Get-ConstValue -Name "YtDlp"
$bundledFfmpegVersion = Get-ConstValue -Name "FFmpeg"
$newtonsoftVersion = Get-ConstValue -Name "NewtonsoftJson"

$csprojText = Get-Content -LiteralPath $projectPath -Raw
$targetFrameworkMatch = [Regex]::Match($csprojText, '<TargetFramework>([^<]+)</TargetFramework>')
$targetFramework = if ($targetFrameworkMatch.Success) { $targetFrameworkMatch.Groups[1].Value } else { "unknown" }
$newtonsoftFromProjectMatch = [Regex]::Match($csprojText, 'PackageReference Include="Newtonsoft\.Json" Version="([^"]+)"')
$newtonsoftFromProject = if ($newtonsoftFromProjectMatch.Success) { $newtonsoftFromProjectMatch.Groups[1].Value } else { "unknown" }

if ($newtonsoftFromProject -ne "unknown" -and $newtonsoftFromProject -ne $newtonsoftVersion) {
    Write-Warning "DependencyVersions.cs and csproj Newtonsoft.Json versions differ ($newtonsoftVersion vs $newtonsoftFromProject)"
}

Write-Host "Repo root: $repoRoot"

if (-not $SkipPublish) {
    Write-Host "Publishing app..."
    dotnet publish $projectPath -c $Configuration -r $Runtime --self-contained false -o $publishDir
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE"
    }
}

if (Test-Path -LiteralPath $tempDir) {
    Remove-Item -LiteralPath $tempDir -Recurse -Force
}
New-Item -ItemType Directory -Path $tempDir | Out-Null

try {
    $ytDlpUrl = "https://github.com/yt-dlp/yt-dlp/releases/download/$bundledYtDlpVersion/yt-dlp.exe"
    $ytDlpPath = Join-Path $publishDir "yt-dlp.exe"
    Write-Host "Downloading pinned yt-dlp: $bundledYtDlpVersion"
    Invoke-WebRequest -Uri $ytDlpUrl -OutFile $ytDlpPath -UseBasicParsing

    $ffmpegZip = Join-Path $tempDir "ffmpeg-release-essentials.zip"
    $ffmpegExtract = Join-Path $tempDir "ffmpeg"
    $ffmpegUrl = "https://www.gyan.dev/ffmpeg/builds/packages/ffmpeg-$bundledFfmpegVersion-essentials_build.zip"
    Write-Host "Downloading pinned FFmpeg essentials: $bundledFfmpegVersion"
    Invoke-WebRequest -Uri $ffmpegUrl -OutFile $ffmpegZip -UseBasicParsing

    Write-Host "Extracting FFmpeg..."
    Expand-Archive -LiteralPath $ffmpegZip -DestinationPath $ffmpegExtract -Force

    $ffmpegExe = Get-ChildItem -Path $ffmpegExtract -Recurse -Filter "ffmpeg.exe" | Select-Object -First 1
    if (-not $ffmpegExe) {
        throw "Could not locate ffmpeg.exe after extraction"
    }

    $ffmpegBinDir = Split-Path -Parent $ffmpegExe.FullName
    foreach ($tool in @("ffmpeg.exe", "ffprobe.exe", "ffplay.exe")) {
        $source = Join-Path $ffmpegBinDir $tool
        if (-not (Test-Path -LiteralPath $source)) {
            throw "Missing $tool in FFmpeg bundle"
        }

        $target = Join-Path $publishDir $tool
        Copy-Item -LiteralPath $source -Destination $target -Force
    }

    $ytDlpActualVersion = (& $ytDlpPath --version 2>$null | Select-Object -First 1)
    $ffmpegActualVersion = (& (Join-Path $publishDir "ffmpeg.exe") -version 2>$null | Select-Object -First 1)

    $summary = [ordered]@{
        SourceDeclaredVersions = [ordered]@{
            YtDlp = $bundledYtDlpVersion
            FFmpeg = $bundledFfmpegVersion
            NewtonsoftJson = $newtonsoftVersion
            TargetFramework = $targetFramework
        }
        ProjectFileVersions = [ordered]@{
            NewtonsoftJson = $newtonsoftFromProject
            TargetFramework = $targetFramework
        }
        DownloadSources = [ordered]@{
            YtDlp = $ytDlpUrl
            FFmpeg = $ffmpegUrl
        }
        DetectedBinaryVersions = [ordered]@{
            YtDlp = if ($ytDlpActualVersion) { $ytDlpActualVersion.Trim() } else { "unknown" }
            FFmpeg = if ($ffmpegActualVersion) { $ffmpegActualVersion.Trim() } else { "unknown" }
        }
        PublishDir = $publishDir
        BundledTools = @("yt-dlp.exe", "ffmpeg.exe", "ffprobe.exe", "ffplay.exe")
        GeneratedAtUtc = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    }

    $summaryPath = Join-Path $publishDir "bundled-dependencies.json"
    $summary | ConvertTo-Json -Depth 4 | Set-Content -Path $summaryPath -Encoding UTF8

    Write-Host "Bundled publish is ready."
    Get-ChildItem -LiteralPath $publishDir -File |
        Where-Object { $_.Name -in @("yt-dlp.exe", "ffmpeg.exe", "ffprobe.exe", "ffplay.exe") } |
        Select-Object Name, @{ Name = "SizeMB"; Expression = { [Math]::Round($_.Length / 1MB, 2) } } |
        Format-Table -AutoSize
}
finally {
    if (Test-Path -LiteralPath $tempDir) {
        Remove-Item -LiteralPath $tempDir -Recurse -Force
    }
}
