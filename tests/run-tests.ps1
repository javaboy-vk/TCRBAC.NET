param(
    [string] $Configuration = "Debug",
    [switch] $Reset,
    [switch] $SkipTestRun,
    [switch] $SkipAllureReport,
    [switch] $OpenAllureReport
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$resultDirectory = Join-Path $repoRoot "tests\TestResults"
$trxPath = Join-Path $resultDirectory "unit-tests.trx"
$allureResultsDirectory = Join-Path $repoRoot "tests\AllureResults"
$allureReportDirectory = Join-Path $repoRoot "tests\AllureReport"
$siteTestDirectory = Join-Path $repoRoot "site\tests"
$siteResultDirectory = Join-Path $siteTestDirectory "results"
$siteAllureReportDirectory = Join-Path $siteTestDirectory "allure-report"
$solution = Join-Path $repoRoot "TCRBAC.NET.sln"
$converter = Join-Path $PSScriptRoot "Convert-TrxToAllure.ps1"

function Reset-TestArtifacts {
    $paths = @(
        $resultDirectory,
        $allureResultsDirectory,
        $allureReportDirectory,
        $siteResultDirectory,
        $siteAllureReportDirectory
    )

    foreach ($path in $paths) {
        if (Test-Path -LiteralPath $path) {
            Remove-Item -LiteralPath $path -Recurse -Force
        }
    }
}

function Copy-DirectoryContent {
    param(
        [string] $Source,
        [string] $Destination
    )

    if (-not (Test-Path -LiteralPath $Source)) {
        return
    }

    New-Item -ItemType Directory -Force -Path $Destination | Out-Null
    Get-ChildItem -LiteralPath $Source -Force | Copy-Item -Destination $Destination -Recurse -Force
}

if ($Reset) {
    Reset-TestArtifacts
}

if ($SkipTestRun -and $SkipAllureReport) {
    Write-Host "Test results were cleaned up. The test suite is ready to run again."
    return
}

New-Item -ItemType Directory -Force -Path $resultDirectory | Out-Null
New-Item -ItemType Directory -Force -Path $allureResultsDirectory | Out-Null

if (-not $SkipTestRun) {
    dotnet test $solution `
        --configuration $Configuration `
        --logger "trx;LogFileName=unit-tests.trx" `
        --results-directory $resultDirectory
}

if (-not (Test-Path -LiteralPath $trxPath)) {
    throw "TRX result file was not found at $trxPath"
}

& $converter -TrxPath $trxPath -OutputDirectory $allureResultsDirectory

New-Item -ItemType Directory -Force -Path $siteResultDirectory | Out-Null
Copy-Item -LiteralPath $trxPath -Destination (Join-Path $siteResultDirectory "unit-tests.trx") -Force

if (-not $SkipAllureReport) {
    if (Test-Path -LiteralPath $allureReportDirectory) {
        Remove-Item -LiteralPath $allureReportDirectory -Recurse -Force
    }

    $allureCommand = Get-Command allure -ErrorAction SilentlyContinue
    if ($allureCommand) {
        & allure generate $allureResultsDirectory -o $allureReportDirectory
    }
    else {
        & npx allure generate $allureResultsDirectory -o $allureReportDirectory
    }

    if ($LASTEXITCODE -ne 0) {
        throw "Allure report generation failed with exit code $LASTEXITCODE"
    }

    Copy-DirectoryContent -Source $allureReportDirectory -Destination $siteAllureReportDirectory

    if ($OpenAllureReport) {
        Start-Process (Join-Path $allureReportDirectory "index.html")
    }
}

Write-Host "TRX results written to $trxPath"
Write-Host "Allure results written to $allureResultsDirectory"
if (Test-Path -LiteralPath (Join-Path $allureReportDirectory "index.html")) {
    Write-Host "Allure report written to $allureReportDirectory\index.html"
    Write-Host "Documentation-site copy written to $siteAllureReportDirectory\index.html"
}
