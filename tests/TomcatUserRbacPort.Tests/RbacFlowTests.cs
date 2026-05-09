using com.mag.dapi.security.credentials;
using com.mag.dapi.security.rbac;
using com.mag.dapi.security.realms;
using com.mag.dapi.security.users;

namespace TomcatUserRbacPort.Tests;

public sealed class RbacFlowTests
{
    [Fact]
    public void FromXml_LoadsUsersGroupsAndEffectiveRoles()
    {
        var database = MemoryUserDatabase.FromXml("""
            <tomcat-users>
              <role rolename="manager-gui" />
              <role rolename="admin-gui" />
              <group groupname="admins" roles="admin-gui" />
              <user username="alice" password="secret" roles="manager-gui" groups="admins" />
            </tomcat-users>
            """);

        var user = database.FindUser("alice");

        Assert.NotNull(user);
        Assert.Equal(new[] { "manager-gui", "admin-gui" }, database.GetEffectiveRoles(user));
    }

    [Fact]
    public void Load_ReadsNamespacedTomcatUsersXml()
    {
        var xmlPath = Path.Combine(TestPaths.RepositoryRoot, "examples", "tomcat", "tomcat-users.xml");

        var database = MemoryUserDatabase.Load(xmlPath);

        Assert.NotNull(database.FindUser("tomcat"));
        Assert.NotNull(database.FindUser("both"));
        Assert.True(database.Roles.ContainsKey("tomcat"));
        Assert.True(database.Roles.ContainsKey("role1"));
    }

    [Fact]
    public void Authenticate_ReturnsPrincipalForValidCredentials()
    {
        var database = MemoryUserDatabase.FromXml("""
            <tomcat-users>
              <group groupname="admins" roles="admin-gui" />
              <user username="alice" password="secret" roles="manager-gui" groups="admins" />
            </tomcat-users>
            """);
        var realm = new UserDatabaseRealm(database, new PlainTextCredentialHandler());

        var principal = realm.Authenticate("alice", "secret");

        Assert.NotNull(principal);
        Assert.Equal("alice", principal.Name);
        Assert.True(principal.IsInRole("manager-gui"));
        Assert.True(principal.IsInRole("admin-gui"));
    }

    [Fact]
    public void Authenticate_ReturnsNullForInvalidPassword()
    {
        var database = MemoryUserDatabase.FromXml("""
            <tomcat-users>
              <user username="alice" password="secret" roles="manager-gui" />
            </tomcat-users>
            """);
        var realm = new UserDatabaseRealm(database, new PlainTextCredentialHandler());

        Assert.Null(realm.Authenticate("alice", "wrong-password"));
    }

    [Fact]
    public void IsAllowed_RequiresMatchingRoleForProtectedPath()
    {
        var principal = new Principal("alice", new[] { "manager-gui" });
        var constraints = new[]
        {
            new SecurityConstraint("/manager/*", new[] { "manager-gui" }, new[] { "GET" })
        };

        Assert.True(RbacAuthorizer.IsAllowed(principal, "/manager/html", "GET", constraints));
        Assert.False(RbacAuthorizer.IsAllowed(null, "/manager/html", "GET", constraints));
        Assert.True(RbacAuthorizer.IsAllowed(null, "/public", "GET", constraints));
    }
}
