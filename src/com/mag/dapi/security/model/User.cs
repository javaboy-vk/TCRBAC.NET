// ============================================================
// File Name:     User.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Represents an XML-backed application user.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using log4net;

namespace com.mag.dapi.security.model;

/// <summary>
/// Represents a user loaded from a Tomcat-style user XML file.
/// </summary>
public sealed class User
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(User));
    private readonly HashSet<string> _roles = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _groups = new(StringComparer.OrdinalIgnoreCase);

    public User(string username, string passwordCredential, string? fullName = null)
    {
        Username = string.IsNullOrWhiteSpace(username)
            ? throw new ArgumentException("Username is required.", nameof(username))
            : username.Trim();
        PasswordCredential = passwordCredential ?? throw new ArgumentNullException(nameof(passwordCredential));
        FullName = fullName;
        Log.Info($"Created user '{Username}'.");
    }

    /// <summary>
    /// Login name from the XML username attribute.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Stored password value. This may be clear text, digest text, or another supported credential format.
    /// </summary>
    public string PasswordCredential { get; }

    /// <summary>
    /// Optional display name.
    /// </summary>
    public string? FullName { get; }

    /// <summary>
    /// Direct role assignments from the XML roles attribute.
    /// </summary>
    public IReadOnlyCollection<string> Roles => _roles;

    /// <summary>
    /// Group memberships from the XML groups attribute.
    /// </summary>
    public IReadOnlyCollection<string> Groups => _groups;

    public void AddRole(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var normalizedRoleName = roleName.Trim();
            if (_roles.Add(normalizedRoleName))
            {
                Log.Info($"Added direct role '{normalizedRoleName}' to user '{Username}'.");
            }
            else
            {
                Log.Info($"Skipped duplicate direct role '{normalizedRoleName}' for user '{Username}'.");
            }
        }
    }

    public void AddGroup(string groupName)
    {
        if (!string.IsNullOrWhiteSpace(groupName))
        {
            var normalizedGroupName = groupName.Trim();
            if (_groups.Add(normalizedGroupName))
            {
                Log.Info($"Added group '{normalizedGroupName}' to user '{Username}'.");
            }
            else
            {
                Log.Info($"Skipped duplicate group '{normalizedGroupName}' for user '{Username}'.");
            }
        }
    }
}
