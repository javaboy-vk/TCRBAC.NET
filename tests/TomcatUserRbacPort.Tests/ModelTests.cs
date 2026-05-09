using com.mag.dapi.security.model;

namespace TomcatUserRbacPort.Tests;

public sealed class ModelTests
{
    [Fact]
    public void Role_TrimsNameAndKeepsDescription()
    {
        var role = new Role(" manager-gui ", "Manager UI access");

        Assert.Equal("manager-gui", role.Name);
        Assert.Equal("Manager UI access", role.Description);
        Assert.Equal("manager-gui", role.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Role_RejectsMissingName(string name)
    {
        Assert.Throws<ArgumentException>(() => new Role(name));
    }

    [Fact]
    public void Group_AddRole_TrimsAndDeduplicatesCaseInsensitively()
    {
        var group = new Group(" admins ");

        group.AddRole(" manager-gui ");
        group.AddRole("MANAGER-GUI");
        group.AddRole("");

        Assert.Equal("admins", group.Name);
        Assert.Single(group.Roles);
        Assert.Contains("manager-gui", group.Roles);
    }

    [Fact]
    public void Group_RejectsMissingName()
    {
        Assert.Throws<ArgumentException>(() => new Group(" "));
    }

    [Fact]
    public void User_AddRoleAndGroup_TrimsAndDeduplicatesCaseInsensitively()
    {
        var user = new User(" alice ", "secret", "Alice Example");

        user.AddRole(" manager-gui ");
        user.AddRole("MANAGER-GUI");
        user.AddGroup(" admins ");
        user.AddGroup("ADMINS");

        Assert.Equal("alice", user.Username);
        Assert.Equal("secret", user.PasswordCredential);
        Assert.Equal("Alice Example", user.FullName);
        Assert.Single(user.Roles);
        Assert.Single(user.Groups);
        Assert.Contains("manager-gui", user.Roles);
        Assert.Contains("admins", user.Groups);
    }

    [Fact]
    public void User_RejectsMissingUsername()
    {
        Assert.Throws<ArgumentException>(() => new User("", "secret"));
    }

    [Fact]
    public void User_RejectsNullPasswordCredential()
    {
        Assert.Throws<ArgumentNullException>(() => new User("alice", null!));
    }
}
