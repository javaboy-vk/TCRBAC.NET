# C# XML Documentation Comments Guide

## Summary

C# does not use Javadoc directly. Its closest built-in equivalent is XML documentation comments, written with triple-slash comments: `///`.

These comments are attached directly to classes, interfaces, methods, properties, constructors, and other public members.

## Common XML documentation tags

| Tag | Purpose |
|---|---|
| `<summary>` | Main description of a type or member. |
| `<remarks>` | Additional details, caveats, or implementation notes. |
| `<param>` | Documents a method parameter. |
| `<returns>` | Documents a method return value. |
| `<exception>` | Documents an exception that may be thrown. |
| `<example>` | Provides usage examples. |
| `<see>` | Links to another type/member inline. |
| `<seealso>` | Adds a related-reference link. |
| `<inheritdoc />` | Inherits documentation from an interface or base member. |

## Example

```csharp
/// <summary>
/// Returns true when the principal has the required role.
/// </summary>
/// <param name="principal">The authenticated user principal.</param>
/// <param name="requiredRole">The required role name.</param>
/// <returns>True when the role is present; otherwise false.</returns>
public bool HasRole(DapiPrincipal principal, string requiredRole)
```

## Compiler setting

Add this to the `.csproj` file:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

## Documentation site generation

DocFX can generate a static documentation site from the C# project and Markdown content.

Typical commands:

```powershell
dotnet tool install -g docfx
docfx metadata docfx.json
docfx build docfx.json
```
