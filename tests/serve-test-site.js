const http = require("http");
const fs = require("fs");
const path = require("path");
const { spawn } = require("child_process");

const args = process.argv.slice(2);
const options = {
  port: 8080,
  configuration: "Debug",
};

for (let i = 0; i < args.length; i += 1) {
  if (args[i] === "--port" && args[i + 1]) {
    options.port = Number(args[i + 1]);
    i += 1;
  } else if (args[i] === "--configuration" && args[i + 1]) {
    options.configuration = args[i + 1];
    i += 1;
  }
}

if (!Number.isInteger(options.port) || options.port < 1 || options.port > 65535) {
  throw new Error(`Invalid port ${options.port}. Use a TCP port from 1 through 65535.`);
}

const repoRoot = path.resolve(__dirname, "..");
const siteRoot = path.join(repoRoot, "site");
const runTestsScript = path.join(__dirname, "run-tests.ps1");

if (!fs.existsSync(path.join(siteRoot, "index.html"))) {
  throw new Error(`The documentation site was not found at ${siteRoot}. Run docfx build from the repository root first.`);
}

const contentTypes = {
  ".html": "text/html; charset=utf-8",
  ".css": "text/css; charset=utf-8",
  ".js": "application/javascript; charset=utf-8",
  ".json": "application/json; charset=utf-8",
  ".svg": "image/svg+xml",
  ".png": "image/png",
  ".ico": "image/x-icon",
  ".trx": "application/xml; charset=utf-8",
};

function writeText(response, statusCode, text, contentType = "text/plain; charset=utf-8") {
  response.writeHead(statusCode, {
    "Access-Control-Allow-Origin": "*",
    "Access-Control-Allow-Methods": "GET, POST, OPTIONS",
    "Access-Control-Allow-Headers": "Content-Type",
    "Content-Type": contentType,
  });
  response.end(text);
}

function getTestStatus() {
  return {
    hasAllureReport: fs.existsSync(path.join(siteRoot, "tests", "allure-report", "index.html")),
    hasCoverageReport: fs.existsSync(path.join(siteRoot, "tests", "coverage-report", "index.html")),
    hasTrxResults: fs.existsSync(path.join(siteRoot, "tests", "results", "unit-tests.trx")),
  };
}

function resolveSitePath(urlPath) {
  let relativePath = decodeURIComponent(urlPath.replace(/^\/+/, ""));
  if (!relativePath) {
    relativePath = "index.html";
  } else if (relativePath.endsWith("/")) {
    relativePath = path.join(relativePath, "index.html");
  }

  const fullPath = path.resolve(siteRoot, relativePath);
  if (!fullPath.toLowerCase().startsWith(siteRoot.toLowerCase())) {
    return null;
  }

  return fullPath;
}

function runTests(resetOnly) {
  return new Promise((resolve) => {
    const commandArgs = [
      "-NoProfile",
      "-ExecutionPolicy",
      "Bypass",
      "-File",
      runTestsScript,
      "-Configuration",
      options.configuration,
      "-Reset",
    ];

    if (resetOnly) {
      commandArgs.push("-SkipTestRun", "-SkipAllureReport", "-SkipCoverageReport");
    }

    const child = spawn("powershell.exe", commandArgs, {
      cwd: repoRoot,
      windowsHide: true,
    });

    let output = "";
    child.stdout.on("data", (chunk) => {
      output += chunk.toString();
    });
    child.stderr.on("data", (chunk) => {
      output += chunk.toString();
    });
    child.on("close", (exitCode) => {
      resolve({ exitCode, output: output.trim() });
    });
  });
}

const server = http.createServer(async (request, response) => {
  try {
    if (request.method === "OPTIONS") {
      writeText(response, 204, "");
      return;
    }

    if (request.method === "GET" && request.url === "/__tests/status") {
      writeText(response, 200, JSON.stringify(getTestStatus()), "application/json; charset=utf-8");
      return;
    }

    if (request.method === "POST" && request.url === "/__tests/reset") {
      const result = await runTests(true);
      writeText(
        response,
        result.exitCode === 0 ? 200 : 500,
        JSON.stringify({ ...result, ...getTestStatus() }),
        "application/json; charset=utf-8"
      );
      return;
    }

    if (request.method === "POST" && request.url === "/__tests/run") {
      const result = await runTests(false);
      writeText(
        response,
        result.exitCode === 0 ? 200 : 500,
        JSON.stringify({ ...result, ...getTestStatus() }),
        "application/json; charset=utf-8"
      );
      return;
    }

    const filePath = resolveSitePath(new URL(request.url, `http://localhost:${options.port}`).pathname);
    if (!filePath || !fs.existsSync(filePath) || !fs.statSync(filePath).isFile()) {
      writeText(response, 404, "Not found");
      return;
    }

    response.writeHead(200, {
      "Access-Control-Allow-Origin": "*",
      "Content-Type": contentTypes[path.extname(filePath).toLowerCase()] || "application/octet-stream",
    });
    fs.createReadStream(filePath).pipe(response);
  } catch (error) {
    writeText(response, 500, error.stack || String(error));
  }
});

server.on("error", (error) => {
  if (error.code === "EADDRINUSE") {
    console.error(`Port ${options.port} is already in use. Stop the existing documentation server on that port, then run this script again.`);
    process.exit(1);
  }

  console.error(error.stack || String(error));
  process.exit(1);
});

server.listen(options.port, "localhost", () => {
  console.log(`Serving TCRBAC.NET documentation at http://localhost:${options.port}/`);
  console.log(`Open http://localhost:${options.port}/tests/index.html to reset or rerun tests from the Tests page.`);
  console.log("Press Ctrl+C to stop.");
});
