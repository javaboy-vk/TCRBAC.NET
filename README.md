# TCRBAC.NET

TCRBAC.NET is a small C#/.NET Framework 4.8 port inspired by Apache Tomcat's `tomcat-users.xml` user database, realm authentication, credential handling, and role-based access control flow.

It is not a byte-for-byte translation of Apache Tomcat. The goal is a compact, readable library that keeps the Tomcat-style security concepts familiar while fitting a C# project layout.

Root namespace: `com.mag.dapi.*`

## Main API

- `com.mag.dapi.security.users.MemoryUserDatabase`
- `com.mag.dapi.security.realms.UserDatabaseRealm`
- `com.mag.dapi.security.credentials.ICredentialHandler`
- `com.mag.dapi.security.rbac.RbacAuthorizer`

## Repository Layout

```text
TCRBAC.NET.sln
src\TomcatUserRbacPort.csproj
tests\TomcatUserRbacPort.Tests\TomcatUserRbacPort.Tests.csproj
examples\TomcatUserValidator\TomcatUserValidator.csproj
conf\log4net.config
docs\
docs\assets\
mdvault\
```

The repository keeps one project file per buildable unit:

- `src` contains the library.
- `tests` contains the xUnit test project.
- `examples` contains the runnable validator example.
- `mdvault` contains source Markdown for DocFX-generated pages.
- `site`, `docs\site`, `examples\site`, `build`, `bin`, and `obj` are generated outputs and should not be committed.

## Prerequisites

- Windows with .NET Framework 4.8 runtime.
- .NET SDK 8.0 or newer. The repo has `global.json` set to SDK `8.0.100` with `rollForward` enabled.
- DocFX for generated HTML documentation.
- Node.js with npm for the repo-local Allure CLI used by the test report workflow.

Install DocFX if needed:

```powershell
dotnet tool install -g docfx
```

Optional, only when regenerating Mermaid-authored diagrams:

```powershell
npm install -g @mermaid-js/mermaid-cli
```

## Allure Local Setup

Allure is installed as a repo-local npm development dependency. A clean DEV environment does not need a global Allure installation.

Install Node.js first if `node` or `npm` is not available:

```powershell
node --version
npm --version
```

Then install the repository JavaScript tooling from the repo root:

```powershell
npm install
```

This restores the Allure CLI declared in `package.json` into `node_modules` and uses `package-lock.json` to keep the installed version reproducible. Verify the local Allure CLI:

```powershell
npm run allure:version
```

Expected output includes:

```text
3.7.0
```

The test runner calls the local Allure CLI automatically through `npx` when a global `allure` command is not on `PATH`, so developers normally only need `npm install`.

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
npm install
dotnet tool restore
dotnet restore TCRBAC.NET.sln
dotnet build TCRBAC.NET.sln
.\tests\run-tests.ps1 -Reset

dotnet run --project examples\TomcatUserValidator -- tomcat tomcat

docfx build
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

The test runner writes TRX results to `tests\TestResults\unit-tests.trx`, Allure result JSON to `tests\AllureResults`, the generated Allure HTML report to `tests\AllureReport\index.html`, Cobertura coverage XML under `tests\TestResults`, and the generated Coverage HTML report to `tests\CoverageReport\index.html`.

Open the command-line generated reports directly:

```powershell
start .\tests\AllureReport\index.html
start .\tests\CoverageReport\index.html
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
docfx build
```

This updates the generated `site` folder only. It does not start or stop any server.

If the documentation site is already running on port `8080`, refresh `http://localhost:8080`. Use the top navigation links for `Tests`, `Allure`, and `Coverage`:

- `http://localhost:8080/tests/index.html`
- `http://localhost:8080/tests/allure-report/index.html`
- `http://localhost:8080/tests/coverage-report/index.html`

Generate the standalone docs site:

```powershell
docfx docs\docfx.json
docfx serve docs\site
```

Generate the standalone examples site:

```powershell
docfx examples\docfx.json
docfx serve examples\site
```

Useful generated pages:

- `site\index.html`
- `site\docs\developer.html`
- `site\docs\Project-Structure.html`
- `site\docs\api\Api-Flow-Diagram.html`
- `site\docs\diagrams\src-class-diagram.html`
- `site\examples\Example-Flow-Diagram.html`
- `site\tests\index.html`

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

![pygount summary for the project](docs/assets/pygount-summary.svg)
