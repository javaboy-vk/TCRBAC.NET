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
.\tests\run-tests.ps1 -Reset
```

The test assembly is built under `build\tests\TomcatUserRbacPort.Tests\<Configuration>\net48`.

The helper script writes:

- TRX results to `tests\TestResults\unit-tests.trx`
- Allure result JSON to `tests\AllureResults`
- Allure HTML report to `tests\AllureReport\index.html`
- Cobertura coverage XML under `tests\TestResults`
- Coverage HTML report to `tests\CoverageReport\index.html`
- documentation-site copies to `site\tests\results`, `site\tests\allure-report`, and `site\tests\coverage-report`

## View results

- <a href="#" onclick="window.location.href='allure-report/index.html'; return false;">Latest Allure report</a>
- <a href="#" onclick="window.location.href='coverage-report/index.html'; return false;">Latest Coverage report</a>

Latest verified run:

| Total | Passed | Failed | Skipped |
|---:|---:|---:|---:|
| 41 | 41 | 0 | 0 |
