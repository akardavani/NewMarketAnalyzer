[CmdletBinding()]
param(
    [ValidatePattern('^[A-Za-z][A-Za-z0-9_-]*$')][string]$EnvironmentName = 'Local',
    [switch]$SkipSpaBuild
)

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$apiProject = Join-Path $repoRoot 'backend/MarketAnalyzer.Api/MarketAnalyzer.Api.csproj'
$uiProject = Join-Path $repoRoot 'frontend/admin/MarketAnalyzer.Ui.Server/MarketAnalyzer.Ui.Server.csproj'
$apiSettings = Join-Path $repoRoot "backend/MarketAnalyzer.Api/appsettings.$EnvironmentName.json"
$uiSettings = Join-Path $repoRoot "frontend/admin/MarketAnalyzer.Ui.Server/appsettings.$EnvironmentName.json"

if (-not (Test-Path -LiteralPath $apiSettings)) { throw "Missing $apiSettings" }
if (-not (Test-Path -LiteralPath $uiSettings)) { throw "Missing $uiSettings" }

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$output = Join-Path $repoRoot "artifacts/publish/$EnvironmentName/$stamp"
$apiOutput = Join-Path $output 'Api'
$uiOutput = Join-Path $output 'Ui'
New-Item -ItemType Directory -Path $apiOutput, $uiOutput | Out-Null

function Invoke-DotNet([string[]]$Arguments) {
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) { throw "dotnet failed (exit $LASTEXITCODE)" }
}

Invoke-DotNet @('publish', $apiProject, '-c', 'Release', '-f', 'net8.0', '-r', 'win-x64', '--self-contained', 'true', '-o', $apiOutput, "-p:EnvironmentName=$EnvironmentName", '-p:RequireDeploymentSettings=true')
$uiArgs = @('publish', $uiProject, '-c', 'Release', '-f', 'net9.0', '-r', 'win-x64', '--self-contained', 'true', '-o', $uiOutput, "-p:EnvironmentName=$EnvironmentName", '-p:RequireDeploymentSettings=true')
if ($SkipSpaBuild) { $uiArgs += '-p:SkipSpaBuild=true' }
Invoke-DotNet $uiArgs

@(
    "Environment: $EnvironmentName",
    "Created: $(Get-Date -Format o)",
    "API: $apiOutput",
    "UI: $uiOutput"
) | Set-Content -LiteralPath (Join-Path $output 'READY.txt') -Encoding UTF8

Write-Host "Published MarketAnalyzer to $output"
