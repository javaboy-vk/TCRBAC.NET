param(
    [ValidateRange(1, 65535)]
    [int] $Port = 8080,
    [string] $Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

$script = Join-Path $PSScriptRoot "serve-test-site.js"
node $script --port $Port --configuration $Configuration
