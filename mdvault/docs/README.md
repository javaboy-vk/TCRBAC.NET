# Tomcat User RBAC Port – Documentation v1.1

## Purpose

This package contains a small C# port inspired by Apache Tomcat's XML-backed user, credential, and RBAC model.

It includes:

- C# source files under `com.mag.dapi.*` style namespaces.
- UML diagrams for the original Java concept map and the C# port.
- Markdown API documentation.
- A DocFX starter configuration.
- Notes on C# XML documentation comments, the closest equivalent to Java Javadoc.

## C# equivalent of Javadoc

The closest C# equivalent to Java Javadoc is **XML documentation comments**.

Example:

```csharp
/// <summary>
/// Authenticates XML-backed users and produces a principal.
/// </summary>
/// <param name="userName">The supplied user name.</param>
/// <returns>A principal when authentication succeeds; otherwise null.</returns>
public DapiPrincipal? Authenticate(string userName, string password)
```

The C# compiler can emit these comments into an `.xml` documentation file. Tools such as **DocFX** can then generate readable API documentation from the XML comments and source metadata.

## Recommended documentation generation

For a .NET project file, enable XML documentation output:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

Then use DocFX with the included `docfx.json` as a starting point.

## Included UML diagrams

- `docs/diagrams/Tomcat-Java-UML-Portrait-BlueLines-v1.2.png`
- `docs/diagrams/Tomcat-CSharp-UML-Portrait-BlueLines-v1.2.png`

## Version

Artifact: `Tomcat User RBAC Port – Documentation Package – v1.1`  
Author: `vasilis`


## v1.2 Example Configuration

The package now includes `examples/tomcat/tomcat-users.xml`, a default-style Tomcat users XML file. The file keeps sample users commented out, matching Tomcat's secure default behavior.
