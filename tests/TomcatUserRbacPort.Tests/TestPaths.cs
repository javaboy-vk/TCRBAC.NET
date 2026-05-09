using System;
using System.IO;

namespace TomcatUserRbacPort.Tests;

internal static class TestPaths
{
    public static string RepositoryRoot
    {
        get
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "TCRBAC.NET.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException(
                $"Could not find the repository root from test output directory: {AppContext.BaseDirectory}");
        }
    }

    public static string ExampleTomcatUsersXml =>
        Path.Combine(RepositoryRoot, "examples", "tomcat", "tomcat-users.xml");
}
