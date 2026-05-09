// ============================================================
// File Name:     MemoryUserDatabase.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Loads Tomcat-style users, roles, and groups from XML.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using System.Xml.Linq;
using com.mag.dapi.security.model;
using log4net;

namespace com.mag.dapi.security.users;

/// <summary>
/// In-memory user database inspired by Tomcat's MemoryUserDatabase.
/// Supports XML elements: role, group, and user.
/// </summary>
public sealed class MemoryUserDatabase
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(MemoryUserDatabase));
    private readonly Dictionary<string, Role> _roles = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Group> _groups = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, Role> Roles => _roles;
    public IReadOnlyDictionary<string, Group> Groups => _groups;
    public IReadOnlyDictionary<string, User> Users => _users;

    /// <summary>
    /// Loads a Tomcat-style XML user database from disk.
    /// </summary>
    public static MemoryUserDatabase Load(string xmlPath)
    {
        if (string.IsNullOrWhiteSpace(xmlPath)) throw new ArgumentException("XML path is required.", nameof(xmlPath));
        Log.Info($"Loading Tomcat-style user database from '{xmlPath}'.");
        var doc = XDocument.Load(xmlPath, LoadOptions.SetLineInfo);
        return FromDocument(doc);
    }

    /// <summary>
    /// Loads a Tomcat-style XML user database from an XML string.
    /// </summary>
    public static MemoryUserDatabase FromXml(string xml)
    {
        Log.Info("Loading Tomcat-style user database from XML text.");
        return FromDocument(XDocument.Parse(xml));
    }

    private static MemoryUserDatabase FromDocument(XDocument doc)
    {
        var db = new MemoryUserDatabase();
        var root = doc.Root ?? throw new InvalidOperationException("User XML has no root element.");

        foreach (var roleElement in Elements(root, "role"))
        {
            var name = Attr(roleElement, "rolename") ?? Attr(roleElement, "name");
            if (name == null || string.IsNullOrWhiteSpace(name)) continue;
            name = name.Trim();
            db.AddRole(new Role(name, Attr(roleElement, "description")));
        }

        foreach (var groupElement in Elements(root, "group"))
        {
            var name = Attr(groupElement, "groupname") ?? Attr(groupElement, "name");
            if (name == null || string.IsNullOrWhiteSpace(name)) continue;
            name = name.Trim();

            var group = new Group(name, Attr(groupElement, "description"));
            foreach (var roleName in SplitCsv(Attr(groupElement, "roles"))) group.AddRole(roleName);
            db.AddGroup(group);
        }

        foreach (var userElement in Elements(root, "user"))
        {
            var username = Attr(userElement, "username") ?? Attr(userElement, "name");
            var password = Attr(userElement, "password") ?? string.Empty;
            if (username == null || string.IsNullOrWhiteSpace(username)) continue;
            username = username.Trim();

            var user = new User(username, password, Attr(userElement, "fullName") ?? Attr(userElement, "fullname"));
            foreach (var roleName in SplitCsv(Attr(userElement, "roles"))) user.AddRole(roleName);
            foreach (var groupName in SplitCsv(Attr(userElement, "groups"))) user.AddGroup(groupName);
            db.AddUser(user);
        }

        Log.Info($"Loaded user database with {db.Roles.Count} role(s), {db.Groups.Count} group(s), and {db.Users.Count} user(s).");
        return db;
    }

    public void AddRole(Role role)
    {
        _roles[role.Name] = role;
        Log.Info($"Registered role '{role.Name}' in user database.");
    }

    public void AddGroup(Group group)
    {
        _groups[group.Name] = group;
        Log.Info($"Registered group '{group.Name}' in user database.");
    }

    public void AddUser(User user)
    {
        _users[user.Username] = user;
        Log.Info($"Registered user '{user.Username}' in user database.");
    }

    public User? FindUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        var normalizedUsername = username.Trim();
        var found = _users.TryGetValue(normalizedUsername, out var user);
        Log.Info(found
            ? $"Found user '{normalizedUsername}' in user database."
            : $"User '{normalizedUsername}' was not found in user database.");
        return found ? user : null;
    }

    public IEnumerable<string> GetEffectiveRoles(User user)
    {
        var result = new HashSet<string>(user.Roles, StringComparer.OrdinalIgnoreCase);

        foreach (var groupName in user.Groups)
        {
            if (_groups.TryGetValue(groupName, out var group))
            {
                foreach (var roleName in group.Roles) result.Add(roleName);
            }
        }

        Log.Info($"Resolved {result.Count} effective role(s) for user '{user.Username}'.");
        return result;
    }

    private static string? Attr(XElement element, string name) => element.Attribute(name)?.Value;

    private static IEnumerable<XElement> Elements(XElement element, string localName) =>
        element.Elements().Where(child => string.Equals(child.Name.LocalName, localName, StringComparison.Ordinal));

    private static IEnumerable<string> SplitCsv(string? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value)) yield break;
        foreach (var token in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(token => token.Trim()))
        {
            if (!string.IsNullOrWhiteSpace(token)) yield return token;
        }
    }
}
