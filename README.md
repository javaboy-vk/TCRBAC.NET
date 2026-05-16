# TCRBAC.NET

TCRBAC.NET is a small C#/.NET Framework 4.8 port inspired by Apache Tomcat's `tomcat-users.xml` user database, realm authentication, credential handling, and role-based access control flow.

It is not a byte-for-byte translation of Apache Tomcat. The goal is a compact, readable library that keeps the Tomcat-style security concepts familiar while fitting a C# project layout.

## Project Structure

**Start here for the repository map:** [Project Structure](docs/Project-Structure.html)

## Main API

Root namespace: `com.mag.dapi.*`

- `com.mag.dapi.security.users.MemoryUserDatabase`
- `com.mag.dapi.security.realms.UserDatabaseRealm`
- `com.mag.dapi.security.credentials.ICredentialHandler`
- `com.mag.dapi.security.rbac.RbacAuthorizer`

## Prerequisites

- Windows with .NET Framework 4.8 runtime.
- .NET SDK 8.0 or newer. The repo has `global.json` set to SDK `8.0.100` with `rollForward` enabled.
- DocFX for generated HTML documentation.
- Node.js with npm only if the test runner has to install the build-local Allure CLI fallback.

Install DocFX if needed:

```powershell
dotnet tool install -g docfx
```

Optional, only when regenerating Mermaid-authored diagrams:

```powershell
npm install -g @mermaid-js/mermaid-cli
```

## Allure Setup

The test runner first looks for a global `allure` command on `PATH`. If it exists, the repository uses that installation and records it in the Project Structure environment setup section.

If no global `allure` command is found, `tests\run-tests.ps1` installs Allure `3.7.0` under `build\tools\allure` and uses `build\tools\npm-cache` for npm cache files. That keeps the generated tooling inside `build` and avoids root `node_modules`.

Install Node.js first only if the script needs this build-local fallback:

```powershell
node --version
npm --version
```

The fallback installation is automatic when the test runner needs it:

```powershell
.\tests\run-tests.ps1 -Reset
```

When the fallback is used, the command line prints the installation path:

```text
Allure was not found globally. Installing Allure 3.7.0 under <repo>\build\tools\allure
```

The same tool location is also written to the Project Structure page under the machine-specific environment setup section, for example `Hercules environment setup`.

You can install Allure globally yourself if you prefer, but the repository does not require that.

## Coverage Local Setup

Coverage is collected by `coverlet.collector` during `dotnet test` and rendered by ReportGenerator. ReportGenerator is installed as a repo-local .NET tool, so a clean DEV environment does not need a global ReportGenerator installation.

Restore the local .NET tools from the repo root:

```powershell
dotnet tool restore
```

This restores `dotnet-reportgenerator-globaltool` from `.config\dotnet-tools.json`.

## Fresh Checkout Setup

Run these commands from a new checkout:

```powershell
git clone <repository-url> TCRBAC.NET
cd TCRBAC.NET

dotnet --info
dotnet nuget list source
dotnet tool restore
dotnet restore TCRBAC.NET.sln
dotnet build TCRBAC.NET.sln
.\tests\run-tests.ps1 -Reset

dotnet run --project examples\TomcatUserValidator -- tomcat tomcat

docfx metadata docfx.json
docfx build docfx.json
docfx docs\docfx.json
docfx examples\docfx.json
```

This updates the generated files only. It does not start a server. If the documentation site is already running on port `8080`, open:

```text
http://localhost:8080
```

## Dependencies

Dependencies are restored from NuGet. Do not commit local DLLs.

The project uses:

- `log4net` `3.3.1`
- `Microsoft.NETFramework.ReferenceAssemblies.net48`
- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`
- `coverlet.collector`

The repo-local `NuGet.config` clears machine-specific package sources and uses `nuget.org`.

## Build And Test

Common root commands:

```powershell
dotnet restore
dotnet clean
dotnet build
.\tests\run-tests.ps1 -Reset
```

Release build:

```powershell
dotnet build --configuration Release
.\tests\run-tests.ps1 -Configuration Release -Reset
```

Build outputs are written under the repository-level `build` directory:

- `build\TomcatUserRbacPort\<Configuration>\net48`
- `build\examples\TomcatUserValidator\<Configuration>\net48`
- `build\tests\TomcatUserRbacPort.Tests\<Configuration>\net48`

The source, test, and example projects copy `conf\log4net.config` into their output directories as `conf\log4net.config`.

The test runner writes TRX results to `build\tests\TestResults\unit-tests.trx`, Allure result JSON to `build\tests\AllureResults`, the generated Allure HTML report to `build\tests\AllureReport\index.html`, Cobertura coverage XML under `build\tests\TestResults`, and the generated Coverage HTML report to `build\tests\CoverageReport\index.html`.

Open the command-line generated reports directly:

```powershell
start .\build\tests\AllureReport\index.html
start .\build\tests\CoverageReport\index.html
```

## Example

Run the sample Tomcat user validator:

```powershell
dotnet run --project examples\TomcatUserValidator -- tomcat tomcat
dotnet run --project examples\TomcatUserValidator -- tomcat wrong-password
```

The example reads `examples\tomcat\tomcat-users.xml`, loads users through `MemoryUserDatabase`, authenticates through `UserDatabaseRealm`, and prints the result.

## Documentation

Build the full documentation site:

```powershell
docfx metadata docfx.json
docfx build docfx.json
```

This updates API metadata under `docs\api` and the generated site under `build\site`. It does not start or stop any server.

If the documentation site is already running on port `8080`, refresh `http://localhost:8080`. Use the top navigation links for `Tests`, `Allure`, and `Coverage`:

- `http://localhost:8080/tests/index.html`
- `http://localhost:8080/tests/allure-report/index.html`
- `http://localhost:8080/tests/coverage-report/index.html`

Generate the standalone docs site:

```powershell
docfx docs\docfx.json
docfx serve build\docs-site
```

Generate the standalone examples site:

```powershell
docfx examples\docfx.json
docfx serve build\examples-site
```

Useful generated pages:

- `build\site\index.html`
- `build\site\docs\developer.html`
- `build\site\docs\Project-Structure.html`
- `build\site\docs\api\Api-Flow-Diagram.html`
- `build\site\docs\diagrams\src-class-diagram.html`
- `build\site\examples\Example-Flow-Diagram.html`
- `build\site\tests\index.html`

## Minimal Usage

```csharp
var db = MemoryUserDatabase.Load("tomcat-users.xml");
var realm = new UserDatabaseRealm(db, new PlainTextCredentialHandler());
var principal = realm.Authenticate("admin", "secret");

if (principal != null && RbacAuthorizer.HasAnyRole(principal, "manager-gui", "admin-gui"))
{
    Console.WriteLine("Access granted");
}
```

## Project Summary

```powershell
pygount --format=summary --folders-to-skip "...,.git,build,bin,obj,node_modules,.playwright-mcp,site"
```

![pygount summary for the project](docs/assets/pygount-summary.svg)
