using com.mag.dapi.security.credentials;
using com.mag.dapi.security.rbac;
using com.mag.dapi.security.realms;
using com.mag.dapi.security.users;

namespace TomcatUserRbacPort.Tests;

public sealed class RealmAndAuthorizationTests
{
    [Fact]
    public void Principal_TrimsNameAndMatchesRolesCaseInsensitively()
    {
        var principal = new Principal(" alice ", new[] { "Manager-Gui", "manager-gui", "admin-gui" });

        Assert.Equal("alice", principal.Name);
        Assert.Equal(2, principal.Roles.Count);
        Assert.True(principal.IsInRole(" manager-gui "));
    }

    [Fact]
    public void Principal_RejectsMissingNameAndMissingRoleChecks()
    {
        Assert.Throws<ArgumentException>(() => new Principal(" ", Array.Empty<string>()));

        var principal = new Principal("alice", new[] { "admin-gui" });
        Assert.False(principal.IsInRole(""));
        Assert.False(principal.IsInRole("missing"));
    }

    [Fact]
    public void UserDatabaseRealm_AuthenticatesAndReturnsEffectiveRoles()
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
        Assert.True(principal!.IsInRole("manager-gui"));
        Assert.True(principal.IsInRole("admin-gui"));
    }

    [Fact]
    public void UserDatabaseRealm_RejectsUnknownUserAndInvalidPassword()
    {
        var database = MemoryUserDatabase.FromXml("""
            <tomcat-users>
              <user username="alice" password="secret" />
            </tomcat-users>
            """);
        var realm = new UserDatabaseRealm(database, new PlainTextCredentialHandler());

        Assert.Null(realm.Authenticate("bob", "secret"));
        Assert.Null(realm.Authenticate("alice", "wrong"));
    }

    [Fact]
    public void UserDatabaseRealm_RejectsNullDependencies()
    {
        var database = new MemoryUserDatabase();
        var handler = new PlainTextCredentialHandler();

        Assert.Throws<ArgumentNullException>(() => new UserDatabaseRealm(null!, handler));
        Assert.Throws<ArgumentNullException>(() => new UserDatabaseRealm(database, null!));
    }

    [Fact]
    public void SecurityConstraint_MatchesExactAndPrefixPathsWithMethods()
    {
        var exact = new SecurityConstraint("/manager/html", new[] { "manager-gui" }, new[] { "get" });
        var prefix = new SecurityConstraint("/admin/*", Array.Empty<string>());

        Assert.True(exact.Matches("/manager/html", "GET"));
        Assert.True(prefix.Matches("/admin/users", null));
        Assert.Equal("GET", exact.HttpMethods.Single());
    }

    [Fact]
    public void SecurityConstraint_RejectsMissingPatternWrongPathAndWrongMethod()
    {
        Assert.Throws<ArgumentException>(() => new SecurityConstraint(" ", Array.Empty<string>()));

        var constraint = new SecurityConstraint("/manager/*", new[] { "manager-gui" }, new[] { "POST" });

        Assert.False(constraint.Matches("", "POST"));
        Assert.False(constraint.Matches("/public", "POST"));
        Assert.False(constraint.Matches("/manager/html", "GET"));
    }

    [Fact]
    public void RbacAuthorizer_AllowsExpectedRoleCombinations()
    {
        var principal = new Principal("alice", new[] { "manager-gui", "admin-gui" });

        Assert.True(RbacAuthorizer.HasAnyRole(principal, "missing", "manager-gui"));
        Assert.True(RbacAuthorizer.HasAllRoles(principal, "manager-gui", "admin-gui"));
        Assert.True(RbacAuthorizer.HasAnyRole(principal));
        Assert.True(RbacAuthorizer.HasAllRoles(principal));
    }

    [Fact]
    public void RbacAuthorizer_DeniesMissingPrincipalOrRoles()
    {
        var principal = new Principal("alice", new[] { "manager-gui" });

        Assert.False(RbacAuthorizer.HasAnyRole(null, "manager-gui"));
        Assert.False(RbacAuthorizer.HasAllRoles(null, "manager-gui"));
        Assert.False(RbacAuthorizer.HasAnyRole(principal, "admin-gui"));
        Assert.False(RbacAuthorizer.HasAllRoles(principal, "manager-gui", "admin-gui"));
    }

    [Fact]
    public void RbacAuthorizer_AppliesFirstMatchingConstraint()
    {
        var principal = new Principal("alice", new[] { "manager-gui" });
        var constraints = new[]
        {
            new SecurityConstraint("/manager/*", new[] { "manager-gui" }, new[] { "GET" }),
            new SecurityConstraint("/admin/*", Array.Empty<string>()),
            new SecurityConstraint("/reports/*", new[] { "admin-gui" })
        };

        Assert.True(RbacAuthorizer.IsAllowed(principal, "/manager/html", "GET", constraints));
        Assert.True(RbacAuthorizer.IsAllowed(principal, "/admin/home", "POST", constraints));
        Assert.True(RbacAuthorizer.IsAllowed(null, "/public", "GET", constraints));
        Assert.False(RbacAuthorizer.IsAllowed(null, "/manager/html", "GET", constraints));
        Assert.False(RbacAuthorizer.IsAllowed(principal, "/reports/monthly", "GET", constraints));
    }
}
