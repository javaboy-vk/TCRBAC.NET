# Project Structure

This page describes the source and configuration files and directories in the repository root. Generated and local-only directories are documented separately in the Generated Directories section.

| Path | Type | Purpose |
| --- | --- | --- |
| 📁 `.config` | Directory | Repo-local .NET tool manifest. `dotnet tool restore` reads this folder to restore tools such as ReportGenerator. |
| 📁 `.git` | Directory | Git repository metadata. This is created by Git and is not edited manually during normal development. |
| 📁 `.vscode` | Directory | Shared VS Code tasks, launch profiles, settings, and extension recommendations that support build, test, and debug workflows. |
| 📁 `conf` | Directory | Shared runtime configuration files, including `log4net.config`, copied into build outputs. |
| 📁 `docs` | Directory | Documentation support files, static HTML pages, DocFX configuration for the standalone docs site, generated API metadata, assets, and scripts. |
| 📁 `examples` | Directory | Runnable example projects and sample Tomcat input files. |
| 📁 `mdvault` | Directory | Source Markdown for generated DocFX pages. This keeps authored documentation separate from generated HTML output. |
| 📁 `src` | Directory | Main library source project for `TomcatUserRbacPort`. |
| 📁 `templates` | Directory | Custom DocFX template files used to apply the shared documentation header and color theme. |
| 📁 `tests` | Directory | xUnit test project, test runner scripts, generated test results, Allure report output, and coverage report output. |
| `.gitignore` | File | Defines generated, local, and private files that Git should ignore. |
| `Directory.Build.props` | File | MSBuild settings shared by projects in the repository. |
| `docfx.json` | File | Root DocFX configuration for the full documentation site generated into `site`. |
| `global.json` | File | Selects the .NET SDK used by CLI commands. The default asks for SDK 8.0.100 with feature-band roll-forward. |
| `NuGet.config` | File | Repo-local NuGet source configuration. It clears machine-specific feeds and uses `nuget.org`. |
| `omnisharp.json` | File | OmniSharp/C# editor configuration used by VS Code. |
| `package-lock.json` | File | npm lock file that keeps JavaScript tool restores reproducible. |
| `package.json` | File | npm tooling manifest for JavaScript-based development tools such as Allure. |
| `README.md` | File | Main GitHub-facing project overview and quick-start entry point. |
| `TCRBAC.code-workspace` | File | VS Code workspace file for opening the repository with the intended workspace name and settings. |
| `TCRBAC.NET.sln` | File | Visual Studio/.NET solution containing the library, tests, and example project. |
| `toc.yml` | File | Root DocFX table of contents used by the generated documentation site. |

## Generated Directories

These directories are created by normal commands and should not be treated as source:

| Path | Created By | Purpose |
| --- | --- | --- |
| `.playwright-mcp` | Browser/MCP tooling | Stores local browser automation cache and state. It is machine-local and ignored by Git. |
| `build` | `dotnet build` | Contains compiled library, example, and test binaries under the repository-owned output layout. |
| `docs/site` | `docfx docs\docfx.json` | Standalone generated documentation site for the `docs` area. |
| `examples/site` | `docfx examples\docfx.json` | Standalone generated documentation site for the examples area. |
| `node_modules` | `npm install` | Contains restored npm packages, including the repo-local Allure CLI. |
| `obj` | `dotnet restore` and `dotnet build` | Contains MSBuild intermediate files, generated project assets, and temporary compilation state. |
| `site` | `docfx build docfx.json` | Full generated documentation site served with `docfx serve site --port 8080`. |
| `tests/AllureReport` | `tests\run-tests.ps1` | Generated Allure HTML report for test results. |
| `tests/AllureResults` | `tests\run-tests.ps1` | Intermediate Allure result files converted from TRX test output. |
| `tests/CoverageReport` | `tests\run-tests.ps1` | Generated HTML coverage report from ReportGenerator. |
| `tests/TestResults` | `tests\run-tests.ps1` and `dotnet test` | Raw test outputs, including TRX files and coverage collector output. |

## Main Source Directories

- `src` contains the library project.
- `tests` contains test code and test/report scripts.
- `examples` contains runnable example code and sample Tomcat files.
- `mdvault` contains authored Markdown documentation.
- `docs` contains documentation assets, generated API metadata, static documentation pages, and documentation scripts.
