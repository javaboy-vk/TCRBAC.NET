# TomcatUserValidator Example

`TomcatUserValidator` is a small .NET Framework 4.8 console program that reads <a href="/examples/tomcat/tomcat-users.xml">examples/tomcat/tomcat-users.xml</a> and validates a username/password pair against the users in that file.

## Run

From the repository root:

```powershell
dotnet run --project examples\TomcatUserValidator -- tomcat tomcat
```

The command format is:

```powershell
dotnet run --project examples\TomcatUserValidator -- <username> <password>
```

## Expected Results

Successful authentication:

```text
Authentication succeeded for 'tomcat'.
Roles: tomcat
```

Failed authentication:

```powershell
dotnet run --project examples\TomcatUserValidator -- tomcat wrong-password
```

```text
Authentication failed.
```

## Exit Codes

- `0`: Authentication succeeded.
- `1`: Authentication failed.
- `2`: Invalid usage or <a href="/examples/tomcat/tomcat-users.xml">examples/tomcat/tomcat-users.xml</a> could not be found.

## How It Works

The program:

1. Requires exactly two CLI arguments: username and password.
2. Finds <a href="/examples/tomcat/tomcat-users.xml">examples/tomcat/tomcat-users.xml</a> from the repository root or from the executable output path.
3. Loads users with `MemoryUserDatabase.Load`.
4. Authenticates with `UserDatabaseRealm`.
5. Compares credentials with `PlainTextCredentialHandler`.
6. Prints the authenticated user's effective roles when login succeeds.

The sample XML file uses plain-text passwords only for local testing.

## Debug In VS Code

Use the Run and Debug view and select one of these launch targets:

- `TomcatUserValidator: tomcat/tomcat`
- `TomcatUserValidator: both/both`
- `TomcatUserValidator: failed login`

Each launch target builds `examples/TomcatUserValidator/TomcatUserValidator.csproj` before running the program.
