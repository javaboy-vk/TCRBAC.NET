param(
    [string] $Configuration = "Debug",
    [switch] $Reset,
    [switch] $SkipTestRun,
    [switch] $SkipAllureReport,
    [switch] $SkipCoverageReport,
    [switch] $OpenAllureReport
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$resultDirectory = Join-Path $repoRoot "tests\TestResults"
$trxPath = Join-Path $resultDirectory "unit-tests.trx"
$allureResultsDirectory = Join-Path $repoRoot "tests\AllureResults"
$allureReportDirectory = Join-Path $repoRoot "tests\AllureReport"
$coverageReportDirectory = Join-Path $repoRoot "tests\CoverageReport"
$siteTestDirectory = Join-Path $repoRoot "site\tests"
$siteResultDirectory = Join-Path $siteTestDirectory "results"
$siteAllureReportDirectory = Join-Path $siteTestDirectory "allure-report"
$siteCoverageReportDirectory = Join-Path $siteTestDirectory "coverage-report"
$solution = Join-Path $repoRoot "TCRBAC.NET.sln"
$converter = Join-Path $PSScriptRoot "Convert-TrxToAllure.ps1"

function Reset-TestArtifacts {
    $paths = @(
        $resultDirectory,
        $allureResultsDirectory,
        $allureReportDirectory,
        $coverageReportDirectory,
        $siteResultDirectory,
        $siteAllureReportDirectory,
        $siteCoverageReportDirectory
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

function Add-DocumentationHeaderToReportHtml {
    param(
        [string] $ReportDirectory,
        [ValidateSet("Allure", "Coverage")]
        [string] $CurrentReport
    )

    if (-not (Test-Path -LiteralPath $ReportDirectory)) {
        return
    }

    $htmlFiles = Get-ChildItem -LiteralPath $ReportDirectory -Filter "*.html" -File -ErrorAction SilentlyContinue
    if ($htmlFiles.Count -eq 0) {
        return
    }

    $style = @'
<style id="tcrbac-docs-header-style">
  .tcrbac-docs-header { background: #fff; border-bottom: 1px solid #dee2e6; font-family: system-ui, -apple-system, "Segoe UI", sans-serif; position: relative; z-index: 1000; }
  .tcrbac-docs-nav { align-items: center; display: flex; gap: 1rem; min-height: 56px; padding: 0 24px; }
  .tcrbac-docs-brand { color: #111827; font-size: 1rem; font-weight: 600; margin-right: 1rem; text-decoration: none; white-space: nowrap; }
  .tcrbac-docs-link { color: #374151; font-size: .95rem; text-decoration: none; white-space: nowrap; }
  .tcrbac-docs-link:hover { color: #0d6efd; text-decoration: underline; }
  .tcrbac-docs-link[aria-current="page"] { color: #0d6efd; font-weight: 600; }
  @media (max-width: 720px) { .tcrbac-docs-nav { align-items: flex-start; flex-direction: column; gap: .5rem; padding: 12px 16px; } }
</style>
'@

    $allureCurrent = if ($CurrentReport -eq "Allure") { ' aria-current="page"' } else { "" }
    $coverageCurrent = if ($CurrentReport -eq "Coverage") { ' aria-current="page"' } else { "" }
    $header = @"
<header id="tcrbac-docs-header" class="tcrbac-docs-header">
  <nav class="tcrbac-docs-nav" aria-label="Main navigation">
    <a class="tcrbac-docs-brand" href="../../index.html">TCRBAC.NET</a>
    <a class="tcrbac-docs-link" href="../../index.html">Home</a>
    <a class="tcrbac-docs-link" href="../../docs/index.html">Docs</a>
    <a class="tcrbac-docs-link" href="../../api/index.html">API</a>
    <a class="tcrbac-docs-link" href="../../examples/index.html">Examples</a>
    <a class="tcrbac-docs-link" href="../index.html">Tests</a>
    <a class="tcrbac-docs-link" href="../allure-report/index.html"$allureCurrent>Allure</a>
    <a class="tcrbac-docs-link" href="../coverage-report/index.html"$coverageCurrent>Coverage</a>
  </nav>
</header>
"@

    foreach ($file in $htmlFiles) {
        $html = Get-Content -LiteralPath $file.FullName -Raw
        $html = [regex]::Replace($html, '(?s)<style id="tcrbac-docs-header-style">.*?</style>\s*', "")
        $html = [regex]::Replace($html, '(?s)<header id="tcrbac-docs-header".*?</header>\s*', "")

        if ($html -notmatch 'id="tcrbac-docs-header-style"') {
            $html = [regex]::Replace($html, '</head>', "$style`r`n</head>", 1)
        }

        if ($html -notmatch 'id="tcrbac-docs-header"') {
            $html = [regex]::Replace($html, '(<body\b[^>]*>)', "`$1`r`n$header", 1)
        }

        $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
        [IO.File]::WriteAllText($file.FullName, $html, $utf8NoBom)
    }
}

function Remove-ReportGeneratorPromotionalContent {
    param([string] $ReportDirectory)

    $indexPath = Join-Path $ReportDirectory "index.html"
    if (-not (Test-Path -LiteralPath $indexPath)) {
        return
    }

    $html = Get-Content -LiteralPath $indexPath -Raw
    $patterns = @(
        '(?s)<div class="card">\s*<div class="card-header">Method coverage</div>\s*<div class="card-body">\s*<div class="center">\s*<p>Feature is only available for sponsors</p>\s*<a class="pro-button" href="https://reportgenerator\.io/pro" target="_blank">Upgrade to PRO version</a>\s*</div>\s*</div>\s*</div>',
        '<a class="button" href="https://github\.com/danielpalme/ReportGenerator" title="Star on GitHub"><i class="icon-star"></i>Star</a>',
        '<a class="button" href="https://github\.com/sponsors/danielpalme" title="Become a sponsor"><i class="icon-sponsor"></i>Sponsor</a>'
    )

    $updatedHtml = $html
    foreach ($pattern in $patterns) {
        $updatedHtml = [regex]::Replace($updatedHtml, $pattern, "")
    }

    if ($updatedHtml -ne $html) {
        $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
        [IO.File]::WriteAllText($indexPath, $updatedHtml, $utf8NoBom)
    }
}

if ($Reset) {
    Reset-TestArtifacts
}

if ($SkipTestRun -and $SkipAllureReport -and $SkipCoverageReport) {
    Write-Host "Test results were cleaned up. The test suite is ready to run again."
    return
}

New-Item -ItemType Directory -Force -Path $resultDirectory | Out-Null
New-Item -ItemType Directory -Force -Path $allureResultsDirectory | Out-Null

if (-not $SkipTestRun) {
    dotnet test $solution `
        --configuration $Configuration `
        --logger "trx;LogFileName=unit-tests.trx" `
        --results-directory $resultDirectory `
        --collect:"XPlat Code Coverage"
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

    Add-DocumentationHeaderToReportHtml -ReportDirectory $allureReportDirectory -CurrentReport "Allure"
    Copy-DirectoryContent -Source $allureReportDirectory -Destination $siteAllureReportDirectory

    if ($OpenAllureReport) {
        Start-Process (Join-Path $allureReportDirectory "index.html")
    }
}

$coverageFiles = Get-ChildItem -LiteralPath $resultDirectory -Recurse -Filter "coverage.cobertura.xml" -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch "\\In\\" }
if ($coverageFiles.Count -eq 0) {
    throw "Cobertura coverage file was not found under $resultDirectory"
}

if (-not $SkipCoverageReport) {
    if (Test-Path -LiteralPath $coverageReportDirectory) {
        Remove-Item -LiteralPath $coverageReportDirectory -Recurse -Force
    }

    $reports = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
    & dotnet tool run reportgenerator `
        "-reports:$reports" `
        "-targetdir:$coverageReportDirectory" `
        "-reporttypes:Html;TextSummary" `
        "-assemblyfilters:+TomcatUserRbacPort;-TomcatUserRbacPort.Tests;-TomcatUserValidator"

    if ($LASTEXITCODE -ne 0) {
        throw "Coverage report generation failed with exit code $LASTEXITCODE"
    }

    Remove-ReportGeneratorPromotionalContent -ReportDirectory $coverageReportDirectory
    Add-DocumentationHeaderToReportHtml -ReportDirectory $coverageReportDirectory -CurrentReport "Coverage"
    Copy-DirectoryContent -Source $coverageReportDirectory -Destination $siteCoverageReportDirectory
}

Write-Host "TRX results written to $trxPath"
Write-Host "Allure results written to $allureResultsDirectory"
if (Test-Path -LiteralPath (Join-Path $allureReportDirectory "index.html")) {
    Write-Host "Allure report written to $allureReportDirectory\index.html"
    Write-Host "Documentation-site copy written to $siteAllureReportDirectory\index.html"
}
if (Test-Path -LiteralPath (Join-Path $coverageReportDirectory "index.html")) {
    Write-Host "Coverage report written to $coverageReportDirectory\index.html"
    Write-Host "Documentation-site copy written to $siteCoverageReportDirectory\index.html"
}
