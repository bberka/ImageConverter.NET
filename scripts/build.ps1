# build.ps1
# Automated build and packaging script for ImageConverter.NET targeting multiple platforms.
# Each platform build is published, packaged into a .zip, and placed into the /release directory.

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# --- PATH & PROJECT SETTINGS ---
$scriptDir   = $PSScriptRoot
$projectRoot = Resolve-Path "$scriptDir\.."
$srcDir      = Join-Path $projectRoot "src"
$projectName = "ImageConverter.NET"
$projectFile = Join-Path (Join-Path $srcDir "ImageConverter.NET") "$projectName.csproj"

# --- VERSION SETUP ---
$versionFile = Join-Path $projectRoot "VERSION"
if (-not (Test-Path $versionFile)) {
    Write-Host "ERROR: VERSION file not found at '$versionFile'" -ForegroundColor Red
    exit 1
}
$version = (Get-Content $versionFile).Trim()
if ([string]::IsNullOrWhiteSpace($version)) {
    Write-Host "ERROR: VERSION file is empty." -ForegroundColor Red
    exit 1
}

Write-Host "`n=== Building $projectName v$version ===" -ForegroundColor Cyan

# --- OUTPUT DIRECTORIES ---
$releaseDir = Join-Path $projectRoot "release"
$stagingDir = Join-Path $projectRoot "build_temp"

if (Test-Path $releaseDir) { Remove-Item -Path $releaseDir -Recurse -Force }
if (Test-Path $stagingDir) { Remove-Item -Path $stagingDir -Recurse -Force }
New-Item -ItemType Directory -Path $releaseDir | Out-Null
New-Item -ItemType Directory -Path $stagingDir | Out-Null

# --- BUILD TARGETS ---
$buildTargets = @(
# Windows
    @{ Name = "Windows-x64-SelfContained"; Args = "-r win-x64 --self-contained true"; ZipSlug = "win-x64" },
    @{ Name = "Windows-x86-SelfContained"; Args = "-r win-x86 --self-contained true"; ZipSlug = "win-x86" },
    @{ Name = "Windows-x64-SingleFile"; Args = "-r win-x64 --self-contained true /p:PublishSingleFile=true"; ZipSlug = "win-x64-single" },
    @{ Name = "Windows-x86-SingleFile"; Args = "-r win-x86 --self-contained true /p:PublishSingleFile=true"; ZipSlug = "win-x86-single" },

    # Linux
    @{ Name = "Linux-x64-SelfContained"; Args = "-r linux-x64 --self-contained true"; ZipSlug = "linux-x64" },
    @{ Name = "Linux-arm64-SelfContained"; Args = "-r linux-arm64 --self-contained true"; ZipSlug = "linux-arm64" },

    # macOS
    @{ Name = "macOS-x64-SelfContained"; Args = "-r osx-x64 --self-contained true"; ZipSlug = "osx-x64" },
    @{ Name = "macOS-arm64-SelfContained"; Args = "-r osx-arm64 --self-contained true"; ZipSlug = "osx-arm64" }
)

# --- VERIFY PROJECT FILE ---
if (-not (Test-Path $projectFile)) {
    Write-Host "ERROR: Project file not found: $projectFile" -ForegroundColor Red
    exit 1
}

Write-Host "`n--- Publishing build targets ---" -ForegroundColor Yellow

# --- BUILD LOOP ---
foreach ($target in $buildTargets) {
    $targetName = $target.Name
    $args       = $target.Args
    $slug       = $target.ZipSlug
    $outputDir  = Join-Path $stagingDir $targetName

    Write-Host "`nBuilding for: $targetName" -ForegroundColor Green

    $cmd = "dotnet publish `"$projectFile`" -c Release -o `"$outputDir`" $args"
    Invoke-Expression $cmd

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Build failed for $targetName" -ForegroundColor Red
        exit 1
    }

    # Copy docs if available
    foreach ($docFile in @("README.md", "LICENSE")) {
        $srcPath = Join-Path $projectRoot $docFile
        if (Test-Path $srcPath) {
            Copy-Item $srcPath -Destination $outputDir -Force
        }
    }

    # Create ZIP
    $zipName = "$projectName-v$version-$slug.zip"
    $zipPath = Join-Path $releaseDir $zipName

    Write-Host "Packaging → $zipName"
    Compress-Archive -Path (Join-Path $outputDir "*") -DestinationPath $zipPath -Force
}

# --- CLEANUP ---
Write-Host "`nCleaning up temporary build directory..." -ForegroundColor Yellow
if (Test-Path $stagingDir) {
    Remove-Item -Path $stagingDir -Recurse -Force
}

Write-Host "`n✅ Build complete! Packages are in:" -ForegroundColor Cyan
Write-Host "$releaseDir" -ForegroundColor Green