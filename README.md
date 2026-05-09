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

Install DocFX if needed:

```powershell
dotnet tool install -g docfx
```

Optional, only when regenerating Mermaid-authored diagrams:

```powershell
npm install -g @mermaid-js/mermaid-cli
```

## Fresh Checkout Setup

Run these commands from a new checkout:

```powershell
git clone <repository-url> TCRBAC.NET
cd TCRBAC.NET

dotnet --info
dotnet nuget list source
dotnet restore TCRBAC.NET.sln
dotnet build TCRBAC.NET.sln
dotnet test TCRBAC.NET.sln

dotnet run --project examples\TomcatUserValidator -- tomcat tomcat

docfx build
docfx docs\docfx.json
docfx examples\docfx.json
docfx serve site
```

When `docfx serve site` runs on port `8080`, open:

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
dotnet test
```

Release build:

```powershell
dotnet build --configuration Release
dotnet test --configuration Release
```

Build outputs are written under the repository-level `build` directory:

- `build\TomcatUserRbacPort\<Configuration>\net48`
- `build\examples\TomcatUserValidator\<Configuration>\net48`
- `build\tests\TomcatUserRbacPort.Tests\<Configuration>\net48`

The source, test, and example projects copy `conf\log4net.config` into their output directories as `conf\log4net.config`.

## Example

Run the sample Tomcat user validator:

```powershell
dotnet run --project examples\TomcatUserValidator -- tomcat tomcat
dotnet run --project examples\TomcatUserValidator -- tomcat wrong-password
```

The example reads `examples\tomcat\tomcat-users.xml`, loads users through `MemoryUserDatabase`, authenticates through `UserDatabaseRealm`, and prints the result.

## Documentation

Generate the full documentation site:

```powershell
docfx build
docfx serve site
```

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
