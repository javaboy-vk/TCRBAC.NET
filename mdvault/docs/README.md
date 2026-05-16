# TCRBAC.NET Documentation

## Purpose

This documentation describes the TCRBAC.NET C# port inspired by Apache Tomcat's XML-backed user, credential, realm, and RBAC model.

It includes:

- API reference material generated from the `src` project.
- Command-oriented developer setup and build instructions.
- Testing and debugging guidance.
- SVG diagrams under `docs/assets/`.
- Example documentation for `examples/TomcatUserValidator`.
- Notes on C# XML documentation comments, the closest equivalent to Java Javadoc.

## C# Equivalent Of Javadoc

The closest C# equivalent to Java Javadoc is XML documentation comments.

Example:

```csharp
/// <summary>
/// Authenticates XML-backed users and produces a principal.
/// </summary>
/// <param name="userName">The supplied user name.</param>
/// <returns>A principal when authentication succeeds; otherwise null.</returns>
public Principal? Authenticate(string userName, string password)
```

The C# compiler emits these comments into an XML documentation file. DocFX then combines those comments with source metadata to generate readable HTML API documentation.

## Documentation Generation

Generate the full documentation site from the repository root:

```powershell
docfx metadata docfx.json
docfx build docfx.json
```

This updates API metadata under `docs\api` and the generated site under `build\site`. It does not start a server.

Generate only the standalone docs site:

```powershell
docfx docs\docfx.json
docfx serve build\docs-site
```

Generate only the standalone examples site:

```powershell
docfx examples\docfx.json
docfx serve build\examples-site
```

## Current Diagram Assets

- `docs/assets/api-flow-diagram.svg`
- `docs/assets/example-flow-diagram.svg`
- `docs/assets/src-class-diagram.svg`
- `docs/assets/pygount-summary.svg`

## Version

Artifact: TCRBAC.NET documentation package  
Author: vasilis
