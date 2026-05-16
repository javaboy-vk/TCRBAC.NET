# Project Structure

This page describes the source and configuration files and directories in the repository root. Generated and local-only directories are documented separately in the Generated Directories section.

| Path | Type | Purpose |
| --- | --- | --- |
| `.config` | Directory | Repo-local .NET tool manifest. `dotnet tool restore` reads this folder to restore tools such as ReportGenerator. |
| `.git` | Directory | Git repository metadata. This is created by Git and is not edited manually during normal development. |
| `.vscode` | Directory | Shared VS Code tasks, launch profiles, settings, and extension recommendations that support build, test, and debug workflows. |
| `build` | Directory | Repository-owned generated-output root. Only `build/.gitkeep` is intended to be committed so fresh checkouts have the directory. |
| `conf` | Directory | Shared runtime configuration files, including `log4net.config`, copied into build outputs. |
| `docs` | Directory | Documentation support files, static HTML pages, DocFX configuration for the standalone docs site, API metadata, and assets. |
| `examples` | Directory | Runnable example projects, sample Tomcat input files, and example API metadata. |
| `mdvault` | Directory | Source Markdown for generated DocFX pages. This keeps authored documentation separate from generated HTML output. |
| `src` | Directory | Main library source project for `TomcatUserRbacPort`. |
| `templates` | Directory | Custom DocFX template files used to apply the shared documentation header and color theme. |
| `tests` | Directory | xUnit test project, test/report scripts, and test API metadata. Generated test outputs and reports are written under `build`. |
| `.gitignore` | File | Defines generated, local, and private files that Git should ignore. |
| `Directory.Build.props` | File | MSBuild settings shared by projects in the repository. |
| `docfx.json` | File | Root DocFX configuration for the full documentation site generated into `build/site`. |
| `global.json` | File | Selects the .NET SDK used by CLI commands. The default asks for SDK 8.0.100 with feature-band roll-forward. |
| `NuGet.config` | File | Repo-local NuGet source configuration. It clears machine-specific feeds and uses `nuget.org`. |
| `omnisharp.json` | File | OmniSharp/C# editor configuration used by VS Code. |
| `README.md` | File | Main GitHub-facing project overview and quick-start entry point. |
| `TCRBAC.code-workspace` | File | VS Code workspace file for opening the repository with the intended workspace name and settings. |
| `TCRBAC.NET.sln` | File | Visual Studio/.NET solution containing the library, tests, and example project. |
| `toc.yml` | File | Root DocFX table of contents used by the generated documentation site. |

## Generated Directories

These directories are created by normal commands and should not be treated as source:

| Path | Created By | Purpose |
| --- | --- | --- |
| `\build` | `dotnet build` and repository tooling | Repository-owned generated-output root for compiled binaries, reports, generated sites, and build-local tools. |
| `\build\.playwright-mcp` | Browser/MCP tooling | Local browser automation cache and state when browser validation is used for this repository. |
| `\build\docs-site` | `docfx docs\docfx.json` | Standalone generated documentation site for the `docs` area. |
| `\build\examples` | `dotnet build` | Compiled example binaries under the repository-owned output layout. |
| `\build\examples-site` | `docfx examples\docfx.json` | Standalone generated documentation site for the examples area. |
| `\build\obj` | `dotnet restore` and `dotnet build` | MSBuild intermediate files, generated project assets, and temporary compilation state. |
| `\build\site` | `docfx build docfx.json` | Full generated documentation site served with `docfx serve build\site --port 8080`. |
| `\build\tests\AllureReport` | `tests\run-tests.ps1` | Generated Allure HTML report for test results. |
| `\build\tests\AllureResults` | `tests\run-tests.ps1` | Intermediate Allure result files converted from TRX test output. |
| `\build\tests\CoverageReport` | `tests\run-tests.ps1` | Generated HTML coverage report from ReportGenerator. |
| `\build\tests\TestResults` | `tests\run-tests.ps1` and `dotnet test` | Raw test outputs, including TRX files and coverage collector output. |
| `\build\tools` | Tool setup scripts such as `tests\run-tests.ps1` | Build-local tool installation root used when a required tool is not available globally. |
| `\build\tools\allure` | `tests\run-tests.ps1` | Build-local Allure CLI installation used only when no global `allure` command is found. |
| `\build\tools\npm-cache` | `tests\run-tests.ps1` | Build-local npm cache used by the Allure fallback install so npm cache files stay under `\build`. |
| `\build\TomcatUserRbacPort` | `dotnet build` | Compiled library binaries under the repository-owned output layout. |
| `\build\TomcatUserRbacPort.Tests` | `dotnet build` | Compiled test binaries under the repository-owned output layout. |
| `\build\TomcatUserValidator` | `dotnet build` | Compiled example executable binaries under the repository-owned output layout. |

No generated project directory should be created outside `\build`. If an external tool creates a cache folder in the repository root, move or remove it and use the supported `\build` paths above.

## Hercules environment setup

<!-- environment-setup:start -->

This section is updated by repository setup/test scripts when they install or discover tool dependencies for this machine.

| Tool | Scope | Installed/Resolved Path | Command | Version | Managed By | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| Allure CLI | Build-local | `D:\TCRBAC.NET\build\tools\allure` | `D:\TCRBAC.NET\build\tools\allure\node_modules\.bin\allure.cmd` | 3.7.0 | tests\run-tests.ps1 | Installed under build because no global allure command was found. |
| npm package cache | Build-local | `D:\TCRBAC.NET\build\tools\npm-cache` | `npm install --cache D:\TCRBAC.NET\build\tools\npm-cache` |  | tests\run-tests.ps1 | Used by build-local tool installation so npm cache files stay under build. |
| Playwright MCP cache | Build-local | `D:\TCRBAC.NET\build\.playwright-mcp` | `browser/MCP tooling cache` |  | manual repo cleanup | Moved under build so browser automation state is not created in the repository root. |

<!-- environment-setup:end -->

## Main Source Directories
- `src` contains the library project.
- `tests` contains test code and test/report scripts.
- `examples` contains runnable example code and sample Tomcat files.
- `mdvault` contains authored Markdown documentation.
- `docs` contains documentation assets, static documentation pages, and DocFX configuration.
