# TCRBAC Examples

This folder contains runnable examples and sample Tomcat-style configuration files for the TCRBAC library.

## Contents

- `TomcatUserValidator`: .NET Framework 4.8 console program that validates a username and password against <a href="/examples/tomcat/tomcat-users.xml">tomcat-users.xml</a>.
- <a href="/examples/tomcat/tomcat-users.xml">tomcat/tomcat-users.xml</a>: sample Tomcat-style users file.
- <a href="/examples/tomcat/tomcat-users.xsd">tomcat/tomcat-users.xsd</a>: local schema used by XML editors to validate the sample users file.

## Generate Examples HTML Documentation

From the repository root:

```powershell
docfx examples\docfx.json
```

The generated HTML site is written to:

```text
examples\site
```

Preview it locally:

```powershell
docfx serve examples\site
```
