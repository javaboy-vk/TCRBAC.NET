# Tomcat Example Files v1.2

## File

<a href="/examples/tomcat/tomcat-users.xml">tomcat-users.xml</a>

<a href="/examples/tomcat/tomcat-users.xsd">tomcat-users.xsd</a>

## Purpose

This is the default-style Apache Tomcat <a href="/examples/tomcat/tomcat-users.xml">conf/tomcat-users.xml</a> example file included for local testing and documentation of the C# RBAC port.

The local <a href="/examples/tomcat/tomcat-users.xsd">tomcat-users.xsd</a> file is included so XML editors can validate the sample without reporting that the `tomcat-users` declaration is missing.

## Important security note

The sample users are intentionally commented out. This mirrors Tomcat's secure default behavior: no manager user is active until you explicitly define one.

For local testing, copy this file to your test configuration folder and either:

1. Uncomment and modify one of the sample `<user>` entries, or
2. Add a new test user with a password format supported by the selected C# `ICredentialHandler`.

Do not use `<must-be-changed>` or sample credentials in production.

## CLI validator example

The `examples/TomcatUserValidator` console project reads this <a href="/examples/tomcat/tomcat-users.xml">tomcat-users.xml</a> file and validates a username/password pair:

```powershell
dotnet run --project examples\TomcatUserValidator -- tomcat tomcat
```

The sample file currently includes plain-text users for local testing, so the validator uses `PlainTextCredentialHandler`.

For detailed instructions, see `examples/TomcatUserValidator/README.md`.
