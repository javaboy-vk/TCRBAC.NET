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
  :root { --tcrbac-vscode-blue: #007acc; --tcrbac-vscode-blue-dark: #005a9e; --tcrbac-vscode-orange: #ff8c00; --tcrbac-vscode-orange-soft: rgba(255, 140, 0, 0.14); }
  #tcrbac-docs-header { background: var(--tcrbac-vscode-blue) !important; border-bottom: 1px solid var(--tcrbac-vscode-blue-dark) !important; box-sizing: border-box !important; display: flex !important; align-items: stretch !important; font-family: system-ui, -apple-system, "Segoe UI", sans-serif !important; height: 60px !important; min-height: 60px !important; position: relative !important; width: 100% !important; z-index: 1000 !important; }
  #tcrbac-docs-header * { box-sizing: border-box !important; }
  #tcrbac-docs-header .navbar { align-items: center !important; display: flex !important; flex: 1 1 auto !important; height: 60px !important; min-height: 60px !important; padding: 0 !important; }
  #tcrbac-docs-header .container-xxl { align-items: center !important; display: flex !important; flex-wrap: nowrap !important; height: 60px !important; justify-content: flex-start !important; margin: 0 !important; min-height: 60px !important; padding: 0 12px !important; width: 100% !important; }
  #tcrbac-docs-header .navbar-brand,
  #tcrbac-docs-header .navbar-brand:visited { align-items: center !important; color: #fff !important; display: inline-flex !important; flex: 0 0 auto !important; font-size: 16px !important; font-weight: 400 !important; height: 60px !important; line-height: 60px !important; margin: 0 14px 0 0 !important; max-width: none !important; min-width: 0 !important; padding: 0 !important; text-decoration: none !important; white-space: nowrap !important; }
  #tcrbac-docs-header #logo { display: block !important; flex: 0 0 auto !important; height: 38px !important; margin: 0 6px 0 0 !important; max-height: 38px !important; max-width: 38px !important; width: 38px !important; }
  #tcrbac-docs-header #navpanel { align-items: center !important; display: flex !important; flex: 1 1 auto !important; height: 60px !important; min-height: 60px !important; }
  #tcrbac-docs-header #navbar { align-items: center !important; display: flex !important; flex: 1 1 auto !important; height: 60px !important; justify-content: flex-start !important; min-height: 60px !important; }
  #tcrbac-docs-header .navbar-nav { align-items: center !important; display: flex !important; flex-direction: row !important; flex-wrap: nowrap !important; gap: 16px !important; height: 60px !important; list-style: none !important; margin: 0 !important; padding: 0 !important; }
  #tcrbac-docs-header .nav-item { display: block !important; margin: 0 !important; padding: 0 !important; }
  #tcrbac-docs-header .nav-link,
  #tcrbac-docs-header .nav-link:visited { align-items: center !important; background: transparent !important; border: 0 !important; color: #fff !important; display: inline-flex !important; flex: 0 0 auto !important; font-size: 14px !important; font-weight: 400 !important; height: 60px !important; line-height: 60px !important; margin: 0 !important; padding: 0 !important; text-decoration: none !important; white-space: nowrap !important; }
  #tcrbac-docs-header .nav-link:hover,
  #tcrbac-docs-header .nav-link:focus { color: #fff !important; text-decoration: none !important; }
  #tcrbac-docs-header .nav-link.active,
  #tcrbac-docs-header .nav-link[aria-current="page"] { color: var(--tcrbac-vscode-orange) !important; font-weight: 700 !important; }
  .tcrbac-docs-header + #app { --bg-support-atlas: var(--tcrbac-vscode-orange); --on-support-atlas: var(--tcrbac-vscode-orange); }
  .card-header, tr.header th { background: var(--tcrbac-vscode-blue) !important; color: #fff !important; }
  a, a:visited { color: var(--tcrbac-vscode-blue); }
  a:hover, a:focus { color: var(--tcrbac-vscode-orange); }
  @media (max-width: 720px) { #tcrbac-docs-header { height: auto !important; min-height: 60px !important; } #tcrbac-docs-header .container-xxl { align-items: flex-start !important; flex-direction: column !important; height: auto !important; padding: 8px 12px !important; } #tcrbac-docs-header #navpanel, #tcrbac-docs-header #navbar, #tcrbac-docs-header .navbar-nav { height: auto !important; min-height: 0 !important; } #tcrbac-docs-header .navbar-nav { flex-wrap: wrap !important; gap: 12px !important; padding-bottom: 8px !important; } #tcrbac-docs-header .nav-link { height: 28px !important; line-height: 28px !important; } }
</style>
'@

    $allureCurrent = if ($CurrentReport -eq "Allure") { ' aria-current="page"' } else { "" }
    $coverageCurrent = if ($CurrentReport -eq "Coverage") { ' aria-current="page"' } else { "" }
    $header = @"
<header id="tcrbac-docs-header" class="bg-body border-bottom tcrbac-docs-header">
  <nav id="autocollapse" class="navbar navbar-expand-md" role="navigation">
    <div class="container-xxl flex-nowrap">
      <a class="navbar-brand" href="../../index.html"><img id="logo" class="svg" src="../../logo.svg" alt="TCRBAC.NET">TCRBAC.NET</a>
      <div class="collapse navbar-collapse" id="navpanel">
        <div id="navbar">
          <ul class="navbar-nav">
            <li class="nav-item"><a class="nav-link" href="../../index.html">Home</a></li>
            <li class="nav-item"><a class="nav-link" href="../../docs/index.html">Docs</a></li>
            <li class="nav-item"><a class="nav-link" href="../../api/index.html">API</a></li>
            <li class="nav-item"><a class="nav-link" href="../../examples/index.html">Examples</a></li>
            <li class="nav-item"><a class="nav-link" href="../index.html">Tests</a></li>
            <li class="nav-item"><a class="nav-link" href="../allure-report/index.html"$allureCurrent>Allure</a></li>
            <li class="nav-item"><a class="nav-link" href="../coverage-report/index.html"$coverageCurrent>Coverage</a></li>
          </ul>
        </div>
      </div>
    </div>
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
