# Tests

The test suite uses xUnit and targets the `TomcatUserRbacPort.Tests` project.

## Unit Tests

- [Unit-Tests](xref:TomcatUserRbacPort.Tests)

## Coverage

The unit tests include positive and negative cases for every source class:

| Area | Classes covered |
|---|---|
| Model | `Role`, `Group`, `User` |
| Credentials | `PlainTextCredentialHandler`, `MessageDigestCredentialHandler`, `NestedCredentialHandler` |
| User database | `MemoryUserDatabase` |
| Realm | `Principal`, `UserDatabaseRealm` |
| Authorization | `SecurityConstraint`, `RbacAuthorizer` |
| Example app | `Program` |

## Run the tests

From the repository root:

```powershell
dotnet test tests\TomcatUserRbacPort.Tests\TomcatUserRbacPort.Tests.csproj --logger "trx;LogFileName=unit-tests.trx" --results-directory tests\TestResults
```

The test assembly is built under `build\tests\TomcatUserRbacPort.Tests\<Configuration>\net48`.

Or run the helper script:

```powershell
.\tests\run-tests.ps1
```

## View results

- <a href="results/unit-tests.trx">Latest TRX test results</a>

Latest verified run:

| Total | Passed | Failed | Skipped |
|---:|---:|---:|---:|
| 41 | 41 | 0 | 0 |
