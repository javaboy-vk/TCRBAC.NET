param(
    [string] $Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$resultDirectory = Join-Path $repoRoot "tests\TestResults"
$solution = Join-Path $repoRoot "TCRBAC.NET.sln"

New-Item -ItemType Directory -Force -Path $resultDirectory | Out-Null

dotnet test $solution `
    --configuration $Configuration `
    --logger "trx;LogFileName=unit-tests.trx" `
    --results-directory $resultDirectory

Write-Host "Results written to $resultDirectory\unit-tests.trx"
