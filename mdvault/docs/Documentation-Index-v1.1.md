# Tomcat User RBAC Port – Documentation Index v1.1

## Documentation files

1. `README.md`  
   Overview of the package and documentation approach.

2. `CSharp-XML-Documentation-Guide-v1.1.md`  
   Explanation of the C# equivalent to Javadoc.

3. `api/Api-Reference-v1.1.md`  
   Markdown API reference for the included C# source files.

4. `docfx.json`  
   Starter DocFX configuration.

5. `assets/*.svg`  
   Shared SVG diagrams and documentation assets.

6. `diagrams/src-class-diagram.md`  
   Mermaid UML class diagram for the C# source classes under `src`.

7. `assets/src-class-diagram.svg`  
   Rendered SVG UML class diagram for the C# source classes under `src`.

## Recommended documentation workflow

1. Keep XML documentation comments in the C# source files.
2. Enable XML documentation output in the `.csproj`.
3. Use DocFX to generate a static documentation site.
4. Store generated documentation outside source control unless the team wants published static docs checked in.


## v1.2 Example File Addition

- `examples/tomcat/tomcat-users.xml` — default-style Tomcat users XML file for local testing and documentation.
