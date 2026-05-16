# Testing And Debugging

This project uses xUnit tests in `tests/TomcatUserRbacPort.Tests`. The tests target .NET Framework 4.8 and exercise the RBAC library through the same public APIs used by applications.

## Run Tests From The CLI

From the repository root:

```powershell
dotnet test
```

To run only the test project:

```powershell
dotnet test tests\TomcatUserRbacPort.Tests\TomcatUserRbacPort.Tests.csproj
```

To build without running tests:

```powershell
dotnet build tests\TomcatUserRbacPort.Tests\TomcatUserRbacPort.Tests.csproj
```

## Run Tests From VS Code

Use the Command Palette:

```text
Tasks: Run Task
```

Then choose:

- `Run Tests`
- `Build Tests`

The `Run Tests` task executes:

```powershell
dotnet test tests\TomcatUserRbacPort.Tests\TomcatUserRbacPort.Tests.csproj
```

## Debug Tests From VS Code

Open the Run and Debug view and choose:

```text
Debug Tests: RbacFlowTests
```

Set breakpoints in `RbacFlowTests.cs` or in library files under `src/com/mag/dapi/security`, then start debugging.

## Current Test Coverage

`RbacFlowTests` verifies:

- XML strings load users, groups, and effective roles.
- The namespaced `examples/tomcat/tomcat-users.xml` sample loads correctly.
- Valid credentials produce a `Principal`.
- Invalid passwords return `null`.
- RBAC constraints allow or deny access based on roles.

## API Documentation

Generate the HTML API documentation with:

```powershell
docfx docs\docfx.json
```

Or from VS Code:

```text
Tasks: Run Task -> Build Docs
```

The generated HTML site is written to:

```text
build\docs-site
```

Preview it locally:

```powershell
docfx serve build\docs-site
```

Then open the local URL printed by DocFX.
