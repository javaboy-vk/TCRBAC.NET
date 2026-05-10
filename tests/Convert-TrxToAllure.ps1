param(
    [Parameter(Mandatory = $true)]
    [string] $TrxPath,

    [Parameter(Mandatory = $true)]
    [string] $OutputDirectory,

    [string] $ReportName = "TCRBAC.NET Tests"
)

$ErrorActionPreference = "Stop"

function Convert-ToUnixMilliseconds {
    param([string] $Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return $null
    }

    $date = [DateTimeOffset]::Parse($Value, [Globalization.CultureInfo]::InvariantCulture)
    return [int64]($date.ToUniversalTime() - [DateTimeOffset]::FromUnixTimeMilliseconds(0)).TotalMilliseconds
}

function Get-ChildElement {
    param(
        [System.Xml.XmlNode] $Node,
        [string] $LocalName
    )

    return $Node.ChildNodes | Where-Object { $_.LocalName -eq $LocalName } | Select-Object -First 1
}

function Get-OutcomeStatus {
    param([string] $Outcome)

    switch ($Outcome) {
        "Passed" { "passed" }
        "Failed" { "failed" }
        "NotExecuted" { "skipped" }
        "Warning" { "broken" }
        default { "broken" }
    }
}

function Get-TestDetailsById {
    param([xml] $Trx)

    $details = @{}
    $unitTests = $Trx.GetElementsByTagName("UnitTest")

    foreach ($unitTest in $unitTests) {
        $testMethod = Get-ChildElement -Node $unitTest -LocalName "TestMethod"
        if ($null -eq $testMethod) {
            continue
        }

        $details[$unitTest.id] = @{
            ClassName = [string] $testMethod.className
            MethodName = [string] $testMethod.name
            CodeBase = [string] $testMethod.codeBase
        }
    }

    return $details
}

function Set-Utf8NoBomContent {
    param(
        [string] $Path,
        [string] $Value
    )

    $encoding = New-Object System.Text.UTF8Encoding($false)
    [IO.File]::WriteAllText($Path, $Value, $encoding)
}

$resolvedTrxPath = Resolve-Path $TrxPath
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

[xml] $trx = Get-Content -LiteralPath $resolvedTrxPath
$testDetails = Get-TestDetailsById -Trx $trx
$results = $trx.GetElementsByTagName("UnitTestResult")

$runNode = $trx.DocumentElement
$runName = if ($runNode.name) { [string] $runNode.name } else { $ReportName }

foreach ($result in $results) {
    $details = $testDetails[$result.testId]
    $className = if ($details) { $details.ClassName } else { "" }
    $methodName = if ($details) { $details.MethodName } else { [string] $result.testName }
    $packageName = if ($className -match "^(.*)\.[^.]+$") { $Matches[1] } else { "TCRBAC.NET" }
    $fixtureName = if ($className) { $className } else { "TCRBAC.NET.Tests" }

    $statusDetails = @{}
    $output = Get-ChildElement -Node $result -LocalName "Output"
    if ($output) {
        $errorInfo = Get-ChildElement -Node $output -LocalName "ErrorInfo"
        if ($errorInfo) {
            $message = Get-ChildElement -Node $errorInfo -LocalName "Message"
            $stackTrace = Get-ChildElement -Node $errorInfo -LocalName "StackTrace"
            if ($message) {
                $statusDetails.message = [string] $message.InnerText
            }
            if ($stackTrace) {
                $statusDetails.trace = [string] $stackTrace.InnerText
            }
        }
    }

    $allureResult = [ordered]@{
        uuid = [string] $result.executionId
        name = $methodName
        fullName = [string] $result.testName
        historyId = [string] $result.testId
        testCaseId = [string] $result.testId
        status = Get-OutcomeStatus -Outcome ([string] $result.outcome)
        stage = "finished"
        start = Convert-ToUnixMilliseconds -Value ([string] $result.startTime)
        stop = Convert-ToUnixMilliseconds -Value ([string] $result.endTime)
        labels = @(
            @{ name = "language"; value = "C#" },
            @{ name = "framework"; value = "xUnit" },
            @{ name = "package"; value = $packageName },
            @{ name = "suite"; value = $fixtureName },
            @{ name = "host"; value = [string] $result.computerName },
            @{ name = "testType"; value = "unit" }
        )
    }

    if ($statusDetails.Count -gt 0) {
        $allureResult.statusDetails = $statusDetails
    }

    $jsonPath = Join-Path $OutputDirectory "$($result.executionId)-result.json"
    Set-Utf8NoBomContent -Path $jsonPath -Value ($allureResult | ConvertTo-Json -Depth 10)
}

$environmentPath = Join-Path $OutputDirectory "environment.properties"
@(
    "Project=TCRBAC.NET"
    "Framework=xUnit"
    "TargetFramework=net48"
    "Source=$(([string] $resolvedTrxPath).Replace('\', '/'))"
) | Set-Content -LiteralPath $environmentPath -Encoding ASCII

$executor = [ordered]@{
    name = "tests/run-tests.ps1"
    type = "local"
    reportName = $ReportName
    buildName = $runName
}
Set-Utf8NoBomContent -Path (Join-Path $OutputDirectory "executor.json") -Value ($executor | ConvertTo-Json -Depth 5)

Write-Host "Converted $($results.Count) TRX test results to $OutputDirectory"
