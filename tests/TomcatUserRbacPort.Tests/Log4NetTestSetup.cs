using log4net;
using log4net.Config;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TomcatUserRbacPort.Tests;

internal static class Log4NetTestSetup
{
    [ModuleInitializer]
    public static void Configure()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
        var configFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, "conf", "log4net.config"));

        if (configFile.Exists)
        {
            XmlConfigurator.Configure(logRepository, configFile);
            return;
        }

        BasicConfigurator.Configure(logRepository);
    }
}
