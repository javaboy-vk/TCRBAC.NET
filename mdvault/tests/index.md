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
- documentation-site copies to `site\tests\results` and `site\tests\allure-report`

## View results

- <span id="latest-allure-report"></span>

<script>
(() => {
  const runnerOrigin = window.location.origin;
  const container = document.getElementById("latest-allure-report");
  if (!container) {
    return;
  }

  const link = document.createElement("a");
  link.href = "allure-report/index.html";
  link.textContent = "Latest Allure report";

  const unavailable = document.createElement("span");
  unavailable.textContent = "No Allure report is available. Run the test suite again.";

  async function render() {
    try {
      const response = await fetch(`${runnerOrigin}/__tests/status`);
      if (response.ok) {
        const status = await response.json();
        container.replaceChildren(status.hasAllureReport ? link : unavailable);
        return;
      }
    } catch (_) {
      // Fall back to a plain link when the page is opened without the local runner.
    }

    container.replaceChildren(link);
  }

  render();
})();
</script>

Latest verified run:

| Total | Passed | Failed | Skipped |
|---:|---:|---:|---:|
| 41 | 41 | 0 | 0 |
