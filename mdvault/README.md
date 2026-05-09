# Tomcat User/RBAC C# Port - v1.0

This package is a practical C# port inspired by Apache Tomcat's `tomcat-users.xml` user database, realm authentication, credential handling, and role-based access control flow.

It is not a byte-for-byte source translation of Apache Tomcat. It is intentionally small, readable, and suitable for a little internal project.

Root namespace: `com.mag.dapi.*`

Main entry points:

- `com.mag.dapi.security.users.MemoryUserDatabase`
- `com.mag.dapi.security.realms.UserDatabaseRealm`
- `com.mag.dapi.security.credentials.ICredentialHandler`
- `com.mag.dapi.security.rbac.RbacAuthorizer`

## .NET CLI

This repository targets .NET Framework 4.8 and includes a solution file and a test project, so the normal .NET CLI workflow runs from the repository root:

```powershell
dotnet restore
dotnet clean
dotnet build
dotnet test
```

Binaries are written under the repository-level `build` directory:

- `build\TomcatUserRbacPort\<Configuration>\net48`
- `build\examples\TomcatUserValidator\<Configuration>\net48`
- `build\tests\TomcatUserRbacPort.Tests\<Configuration>\net48`

Project configuration files belong in `conf`. The source, test, and example projects copy `conf\log4net.config` into their outputs as `conf\log4net.config`.

The projects restore log4net from NuGet with `PackageReference Include="log4net" Version="3.3.1"`; do not commit a local `log4net.dll`.

For release builds:

```powershell
dotnet build --configuration Release
dotnet test --configuration Release
```

Run the sample Tomcat user validator:

```powershell
dotnet run --project examples\TomcatUserValidator -- tomcat tomcat
```

Detailed example instructions are in `examples/TomcatUserValidator/README.md`.

Testing, debugging, and API documentation instructions are in `docs/testing-and-debugging.md`.

Generate HTML documentation for only the examples:

```powershell
docfx examples\docfx.json
```

Typical usage:

```csharp
var db = MemoryUserDatabase.Load("tomcat-users.xml");
var realm = new UserDatabaseRealm(db, new PlainTextCredentialHandler());
var principal = realm.Authenticate("admin", "secret");

if (principal != null && RbacAuthorizer.HasAnyRole(principal, "manager-gui", "admin-gui"))
{
    Console.WriteLine("Access granted");
}
```
