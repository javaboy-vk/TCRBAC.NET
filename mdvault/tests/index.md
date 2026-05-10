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

- <span id="latest-allure-report"></span>
- <span id="latest-coverage-report"></span>

<script>
(() => {
  const runnerOrigin = window.location.origin;
  const reports = [
    {
      containerId: "latest-allure-report",
      statusKey: "hasAllureReport",
      href: "allure-report/index.html",
      label: "Latest Allure report",
      unavailable: "No Allure report is available. Run the test suite again."
    },
    {
      containerId: "latest-coverage-report",
      statusKey: "hasCoverageReport",
      href: "coverage-report/index.html",
      label: "Latest Coverage report",
      unavailable: "No Coverage report is available. Run the test suite again."
    }
  ];

  function makeLink(report) {
    const link = document.createElement("a");
    link.href = report.href;
    link.textContent = report.label;
    return link;
  }

  function makeUnavailable(report) {
    const unavailable = document.createElement("span");
    unavailable.textContent = report.unavailable;
    return unavailable;
  }

  async function render() {
    let status = null;
    try {
      const response = await fetch(`${runnerOrigin}/__tests/status`);
      if (response.ok) {
        status = await response.json();
      }
    } catch (_) {
      // Fall back to a plain link when the page is opened without the local runner.
    }

    for (const report of reports) {
      const container = document.getElementById(report.containerId);
      if (!container) {
        continue;
      }

      if (status) {
        container.replaceChildren(status[report.statusKey] ? makeLink(report) : makeUnavailable(report));
      } else {
        container.replaceChildren(makeLink(report));
      }
    }
  }

  render();
})();
</script>

Latest verified run:

| Total | Passed | Failed | Skipped |
|---:|---:|---:|---:|
| 41 | 41 | 0 | 0 |
