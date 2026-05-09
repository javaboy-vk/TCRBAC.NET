using com.mag.dapi.security.model;
using com.mag.dapi.security.users;

namespace TomcatUserRbacPort.Tests;

public sealed class UserDatabaseTests
{
    [Fact]
    public void FromXml_LoadsRolesGroupsUsersAndEffectiveRoles()
    {
        var database = MemoryUserDatabase.FromXml("""
            <tomcat-users>
              <role rolename="manager-gui" />
              <role name="admin-gui" />
              <group groupname="admins" roles="admin-gui, manager-gui" />
              <user username="alice" password="secret" roles="user-gui" groups="admins" />
            </tomcat-users>
            """);

        var user = database.FindUser(" ALICE ");

        Assert.NotNull(user);
        Assert.True(database.Roles.ContainsKey("manager-gui"));
        Assert.True(database.Groups.ContainsKey("admins"));
        Assert.Equal(new[] { "user-gui", "admin-gui", "manager-gui" }, database.GetEffectiveRoles(user!));
    }

    [Fact]
    public void FromXml_IgnoresElementsWithMissingNames()
    {
        var database = MemoryUserDatabase.FromXml("""
            <tomcat-users>
              <role description="missing name" />
              <group roles="admin-gui" />
              <user password="secret" />
            </tomcat-users>
            """);

        Assert.Empty(database.Roles);
        Assert.Empty(database.Groups);
        Assert.Empty(database.Users);
    }

    [Fact]
    public void FromXml_ThrowsForMalformedXml()
    {
        Assert.Throws<System.Xml.XmlException>(() => MemoryUserDatabase.FromXml("<tomcat-users>"));
    }

    [Fact]
    public void Load_RejectsMissingPath()
    {
        Assert.Throws<ArgumentException>(() => MemoryUserDatabase.Load(" "));
    }

    [Fact]
    public void AddMethods_ReplaceExistingItemsCaseInsensitively()
    {
        var database = new MemoryUserDatabase();

        database.AddRole(new Role("admin"));
        database.AddRole(new Role("ADMIN", "replacement"));
        database.AddGroup(new Group("admins"));
        database.AddGroup(new Group("ADMINS", "replacement"));
        database.AddUser(new User("alice", "first"));
        database.AddUser(new User("ALICE", "second"));

        Assert.Single(database.Roles);
        Assert.Single(database.Groups);
        Assert.Single(database.Users);
        Assert.Equal("replacement", database.Roles["admin"].Description);
        Assert.Equal("replacement", database.Groups["admins"].Description);
        Assert.Equal("second", database.FindUser("alice")!.PasswordCredential);
    }
}
