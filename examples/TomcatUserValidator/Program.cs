using com.mag.dapi.security.credentials;
using com.mag.dapi.security.realms;
using com.mag.dapi.security.users;
using log4net;
using log4net.Config;
using System.Reflection;

namespace TomcatUserValidator;

/// <summary>
/// Example command-line program that authenticates a username and password against
/// the sample Tomcat-style users file in <c>examples/tomcat/tomcat-users.xml</c>.
/// </summary>
public static class Program
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
    private const int Success = 0;
    private const int AuthenticationFailed = 1;
    private const int UsageError = 2;

    /// <summary>
    /// Runs the validator. Expects exactly two arguments: username and password.
    /// </summary>
    /// <param name="args">Command-line arguments in the form <c>&lt;username&gt; &lt;password&gt;</c>.</param>
    /// <returns>
    /// 0 when authentication succeeds, 1 when credentials are invalid, and 2 when usage or configuration is invalid.
    /// </returns>
    public static int Main(string[] args)
    {
        ConfigureLogging();
        Log.Info("Starting Tomcat user validator.");

        if (args.Length != 2)
        {
            Log.Info("Validation stopped because the command-line arguments were invalid.");
            Console.Error.WriteLine("Usage: TomcatUserValidator <username> <password>");
            return UsageError;
        }

        var xmlPath = FindTomcatUsersXml();
        if (xmlPath == null)
        {
            Log.Info("Validation stopped because the sample tomcat-users.xml file could not be found.");
            Console.Error.WriteLine("Could not find examples/tomcat/tomcat-users.xml.");
            return UsageError;
        }

        Log.Info($"Using Tomcat users XML file '{xmlPath}'.");
        var database = MemoryUserDatabase.Load(xmlPath);
        var realm = new UserDatabaseRealm(database, new PlainTextCredentialHandler());
        var principal = realm.Authenticate(args[0], args[1]);

        if (principal == null)
        {
            Log.Info($"Authentication failed for user '{args[0]}'.");
            Console.WriteLine("Authentication failed.");
            return AuthenticationFailed;
        }

        Log.Info($"Authentication succeeded for user '{principal.Name}'.");
        Console.WriteLine($"Authentication succeeded for '{principal.Name}'.");
        Console.WriteLine($"Roles: {string.Join(", ", principal.Roles)}");
        return Success;
    }

    /// <summary>
    /// Locates the sample <c>tomcat-users.xml</c> file from either the repository root
    /// or the executable output directory.
    /// </summary>
    /// <returns>The full XML file path when found; otherwise null.</returns>
    public static string? FindTomcatUsersXml()
    {
        var currentDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "examples", "tomcat", "tomcat-users.xml");
        if (File.Exists(currentDirectoryPath))
        {
            Log.Info($"Found tomcat-users.xml from current directory at '{currentDirectoryPath}'.");
            return currentDirectoryPath;
        }

        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "examples", "tomcat", "tomcat-users.xml");
            if (File.Exists(candidate))
            {
                Log.Info($"Found tomcat-users.xml while walking parent directories at '{candidate}'.");
                return candidate;
            }

            directory = directory.Parent;
        }

        Log.Info("Could not locate tomcat-users.xml from the current directory or executable output path.");
        return null;
    }

    private static void ConfigureLogging()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly() ?? typeof(Program).Assembly);
        var configFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, "conf", "log4net.config"));
        if (configFile.Exists)
        {
            XmlConfigurator.Configure(logRepository, configFile);
            Log.Info($"Configured log4net from '{configFile.FullName}'.");
        }
        else
        {
            BasicConfigurator.Configure();
            Log.Info("Configured log4net with the basic console configuration because log4net.config was not found.");
        }
    }
}
