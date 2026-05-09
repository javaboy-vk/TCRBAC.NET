using System.Diagnostics;

namespace TomcatUserRbacPort.Tests;

public sealed class ExampleProgramTests
{
    [Fact]
    public void Main_ReturnsSuccessForValidSampleCredentials()
    {
        var result = RunValidator("tomcat", "tomcat");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Authentication succeeded", result.Output);
    }

    [Fact]
    public void Main_ReturnsAuthenticationFailedForInvalidSampleCredentials()
    {
        var result = RunValidator("tomcat", "wrong-password");

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("Authentication failed", result.Output);
    }

    [Fact]
    public void Main_ReturnsUsageErrorForMissingArguments()
    {
        var result = RunValidator();

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Usage: TomcatUserValidator", result.Error);
    }

    private static CommandResult RunValidator(params string[] args)
    {
        var repoRoot = TestPaths.RepositoryRoot;
        var outputDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        var configuration = outputDirectory.Parent?.Name ?? "Debug";
        var executable = Path.Combine(
            repoRoot,
            "build",
            "examples",
            "TomcatUserValidator",
            configuration,
            "net48",
            "TomcatUserValidator.exe");

        Assert.True(File.Exists(executable), $"Expected validator executable at {executable}.");

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = executable,
            Arguments = string.Join(" ", args.Select(arg => $"\"{arg}\"")),
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        Assert.NotNull(process);
        var output = process!.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30000), "TomcatUserValidator process timed out.");

        return new CommandResult(process.ExitCode, output, error);
    }

    private sealed class CommandResult
    {
        public CommandResult(int exitCode, string output, string error)
        {
            ExitCode = exitCode;
            Output = output;
            Error = error;
        }

        public int ExitCode { get; }

        public string Output { get; }

        public string Error { get; }
    }
}
