param(
    [string[]] $Roots = @("site", "docs\site", "examples\site", "docs\developer.html")
)

$ErrorActionPreference = "Stop"

$icon = '<img class="tomcat-inline-icon" src="https://tomcat.apache.org/res/images/tomcat.png" alt="Apache Tomcat" title="Apache Tomcat" style="height: 16px; vertical-align: text-bottom; margin-left: 4px; margin-right: 2px;">'
$iconPattern = '<img\b[^>]*class="tomcat-inline-icon"[^>]*>'
$skipTags = @("script", "style", "code", "pre", "kbd", "samp", "title", "textarea")

function Add-TomcatIconsToHtml {
    param(
        [string] $Html
    )

    $clean = [regex]::Replace($Html, "\s*$iconPattern\s*", " ", "IgnoreCase")
    $parts = [regex]::Split($clean, "(<[^>]+>)")
    $skipStack = New-Object System.Collections.Generic.Stack[string]
    $output = New-Object System.Text.StringBuilder

    foreach ($part in $parts) {
        if ($part -eq "") {
            continue
        }

        if ($part.StartsWith("<")) {
            if ($part -match '^<\s*/\s*([a-zA-Z0-9]+)\b') {
                $tagName = $matches[1].ToLowerInvariant()
                if ($skipStack.Count -gt 0 -and $skipStack.Peek() -eq $tagName) {
                    [void] $skipStack.Pop()
                }
            } elseif ($part -match '^<\s*([a-zA-Z0-9]+)\b' -and $part -notmatch '/\s*>$') {
                $tagName = $matches[1].ToLowerInvariant()
                if ($skipTags -contains $tagName) {
                    $skipStack.Push($tagName)
                }
            }

            [void] $output.Append($part)
            continue
        }

        if ($skipStack.Count -gt 0) {
            [void] $output.Append($part)
            continue
        }

        $updated = [regex]::Replace($part, "\bTomcat('s)?\b", {
            param($match)
            "$($match.Value)$icon"
        })
        [void] $output.Append($updated)
    }

    return $output.ToString()
}

$files = foreach ($root in $Roots) {
    if (Test-Path -LiteralPath $root -PathType Leaf) {
        Get-Item -LiteralPath $root
    } elseif (Test-Path -LiteralPath $root -PathType Container) {
        Get-ChildItem -LiteralPath $root -Recurse -Filter *.html
    }
}

foreach ($file in $files) {
    $html = Get-Content -LiteralPath $file.FullName -Raw
    $updated = Add-TomcatIconsToHtml -Html $html
    if ($updated -ne $html) {
        Set-Content -LiteralPath $file.FullName -Value $updated -NoNewline
    }
}

Write-Host "Updated $($files.Count) documentation HTML file(s)."
