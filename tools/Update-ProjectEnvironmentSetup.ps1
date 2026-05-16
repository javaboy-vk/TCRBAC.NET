param(
    [Parameter(Mandatory = $true)]
    [string] $ToolName,

    [Parameter(Mandatory = $true)]
    [string] $Scope,

    [Parameter(Mandatory = $true)]
    [string] $InstallPath,

    [Parameter(Mandatory = $true)]
    [string] $Command,

    [string] $Version = "",

    [string] $ManagedBy = "",

    [string] $Notes = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$environmentDirectory = Join-Path $repoRoot "build\environment"
$environmentStatePath = Join-Path $environmentDirectory "setup.json"
$projectStructureMarkdownPath = Join-Path $repoRoot "docs\Project-Structure.md"
$projectStructureHtmlPath = Join-Path $repoRoot "docs\Project-Structure.html"

function Get-MachineTitle {
    $machineName = if ($env:COMPUTERNAME) { $env:COMPUTERNAME } else { "Local" }
    $textInfo = [Globalization.CultureInfo]::InvariantCulture.TextInfo
    return $textInfo.ToTitleCase($machineName.ToLowerInvariant())
}

function Get-EnvironmentRecords {
    if (-not (Test-Path -LiteralPath $environmentStatePath)) {
        return @()
    }

    $content = Get-Content -LiteralPath $environmentStatePath -Raw
    if ([string]::IsNullOrWhiteSpace($content)) {
        return @()
    }

    $parsed = $content | ConvertFrom-Json
    if ($null -eq $parsed) {
        return @()
    }

    $records = New-Object System.Collections.Generic.List[object]
    foreach ($item in @($parsed)) {
        if ($item.PSObject.Properties.Name -contains "ToolName") {
            $records.Add($item)
        }
        elseif ($item.PSObject.Properties.Name -contains "value" -and $item.PSObject.Properties.Name -contains "Count") {
            foreach ($nested in @($item.value)) {
                if ($nested.PSObject.Properties.Name -contains "ToolName") {
                    $records.Add($nested)
                }
            }
        }
    }

    return $records.ToArray()
}

function Save-EnvironmentRecords {
    param([object[]] $Records)

    New-Item -ItemType Directory -Force -Path $environmentDirectory | Out-Null
    $json = $Records | Sort-Object ToolName | ConvertTo-Json -Depth 5
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [IO.File]::WriteAllText($environmentStatePath, "$json`r`n", $utf8NoBom)
}

function ConvertTo-MarkdownValue {
    param([string] $Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return ""
    }

    return $Value.Replace("|", "\|")
}

function ConvertTo-HtmlValue {
    param([string] $Value)

    return [Net.WebUtility]::HtmlEncode($Value)
}

function Build-MarkdownSection {
    param([object[]] $Records)

    $machineTitle = Get-MachineTitle
    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("## $machineTitle environment setup")
    $lines.Add("")
    $lines.Add("<!-- environment-setup:start -->")
    $lines.Add("")
    $lines.Add("This section is updated by repository setup/test scripts when they install or discover tool dependencies for this machine.")
    $lines.Add("")
    $lines.Add("| Tool | Scope | Installed/Resolved Path | Command | Version | Managed By | Notes |")
    $lines.Add("| --- | --- | --- | --- | --- | --- | --- |")

    foreach ($record in ($Records | Sort-Object ToolName)) {
        $installPathValue = ConvertTo-MarkdownValue $record.InstallPath
        $commandValue = ConvertTo-MarkdownValue $record.Command
        $lines.Add("| $(ConvertTo-MarkdownValue $record.ToolName) | $(ConvertTo-MarkdownValue $record.Scope) | ``$installPathValue`` | ``$commandValue`` | $(ConvertTo-MarkdownValue $record.Version) | $(ConvertTo-MarkdownValue $record.ManagedBy) | $(ConvertTo-MarkdownValue $record.Notes) |")
    }

    $lines.Add("")
    $lines.Add("<!-- environment-setup:end -->")
    return ($lines -join "`r`n")
}

function Build-HtmlSection {
    param([object[]] $Records)

    $machineTitle = Get-MachineTitle
    $rows = foreach ($record in ($Records | Sort-Object ToolName)) {
        "          <tr><td>$(ConvertTo-HtmlValue $record.ToolName)</td><td>$(ConvertTo-HtmlValue $record.Scope)</td><td><code>$(ConvertTo-HtmlValue $record.InstallPath)</code></td><td><code>$(ConvertTo-HtmlValue $record.Command)</code></td><td>$(ConvertTo-HtmlValue $record.Version)</td><td>$(ConvertTo-HtmlValue $record.ManagedBy)</td><td>$(ConvertTo-HtmlValue $record.Notes)</td></tr>"
    }

    return @"
    <section>
      <h2>$machineTitle environment setup</h2>
      <!-- environment-setup:start -->
      <p>This section is updated by repository setup/test scripts when they install or discover tool dependencies for this machine.</p>
      <table>
        <thead>
          <tr>
            <th>Tool</th>
            <th>Scope</th>
            <th>Installed/Resolved Path</th>
            <th>Command</th>
            <th>Version</th>
            <th>Managed By</th>
            <th>Notes</th>
          </tr>
        </thead>
        <tbody>
$($rows -join "`r`n")
        </tbody>
      </table>
      <!-- environment-setup:end -->
    </section>
"@
}

function Update-MarkdownProjectStructure {
    param([object[]] $Records)

    $content = Get-Content -LiteralPath $projectStructureMarkdownPath -Raw
    $section = Build-MarkdownSection -Records $Records

    if ($content -match '(?s)## .+? environment setup\s+<!-- environment-setup:start -->.*?<!-- environment-setup:end -->') {
        $content = [regex]::Replace($content, '(?s)## .+? environment setup\s+<!-- environment-setup:start -->.*?<!-- environment-setup:end -->', $section, 1)
    }
    else {
        $content = $content -replace '(?m)^## Generated Directories\s*$', "$section`r`n`r`n## Generated Directories"
    }

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [IO.File]::WriteAllText($projectStructureMarkdownPath, $content, $utf8NoBom)
}

function Update-HtmlProjectStructure {
    param([object[]] $Records)

    $content = Get-Content -LiteralPath $projectStructureHtmlPath -Raw
    $section = Build-HtmlSection -Records $Records

    if ($content -match '(?s)<section>\s*<h2>.+? environment setup</h2>\s*<!-- environment-setup:start -->.*?<!-- environment-setup:end -->\s*</section>') {
        $content = [regex]::Replace($content, '(?s)<section>\s*<h2>.+? environment setup</h2>\s*<!-- environment-setup:start -->.*?<!-- environment-setup:end -->\s*</section>', $section, 1)
    }
    else {
        $content = $content -replace '(?s)(\s*<section>\s*<h2>Generated Directories</h2>)', "`r`n$section`r`n`$1"
    }

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [IO.File]::WriteAllText($projectStructureHtmlPath, $content, $utf8NoBom)
}

$records = @(Get-EnvironmentRecords)
$existing = $records | Where-Object { $_.ToolName -eq $ToolName } | Select-Object -First 1

$record = [ordered]@{
    ToolName = $ToolName
    Scope = $Scope
    InstallPath = $InstallPath
    Command = $Command
    Version = $Version
    ManagedBy = $ManagedBy
    Notes = $Notes
    UpdatedAt = (Get-Date).ToString("s")
}

if ($existing) {
    $records = @($records | Where-Object { $_.ToolName -ne $ToolName })
}

$records += [pscustomobject]$record
Save-EnvironmentRecords -Records $records
Update-MarkdownProjectStructure -Records $records
Update-HtmlProjectStructure -Records $records

Write-Host "Updated Project Structure environment setup for $ToolName on $(Get-MachineTitle)."
