# Tomcat User RBAC Port - Merged Project File v1.1

## What changed

The two separate `.csproj` files were merged into one root-level project file:

```text
TomcatUserRbacPort.csproj
```

This root project compiles all source files under:

```text
src\com\**\*.cs
```

It also enables XML documentation output, so tools such as DocFX can generate Javadoc-style API documentation.

## log4net handling

The projects are configured to restore log4net from NuGet:

```xml
<PackageReference Include="log4net" Version="3.3.1" />
```

Do not commit `log4net.dll` to GitHub.

## Expected root layout

```text
<repo-root>
  TomcatUserRbacPort.csproj
  .gitignore
  src\com\...
  conf\log4net.config
```

## Build commands

```powershell
dotnet restore
dotnet build
```

If the project has no `Program.cs` or `Main` method, change this line in the project file:

```xml
<OutputType>Exe</OutputType>
```

to:

```xml
<OutputType>Library</OutputType>
```
