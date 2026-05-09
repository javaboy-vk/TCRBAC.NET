// ============================================================
// File Name:     Group.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Represents a group that aggregates security roles.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using log4net;

namespace com.mag.dapi.security.model;

/// <summary>
/// Represents a group of roles. In Tomcat, a user can have direct roles and group-derived roles.
/// </summary>
public sealed class Group
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Group));
    private readonly HashSet<string> _roles = new(StringComparer.OrdinalIgnoreCase);

    public Group(string name, string? description = null)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Group name is required.", nameof(name))
            : name.Trim();
        Description = description;
        Log.Info($"Created group '{Name}'.");
    }

    /// <summary>
    /// Group identifier.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Optional group description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Role names assigned to this group.
    /// </summary>
    public IReadOnlyCollection<string> Roles => _roles;

    /// <summary>
    /// Adds a role to the group. Duplicate role names are ignored.
    /// </summary>
    public void AddRole(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var normalizedRoleName = roleName.Trim();
            if (_roles.Add(normalizedRoleName))
            {
                Log.Info($"Added role '{normalizedRoleName}' to group '{Name}'.");
            }
            else
            {
                Log.Info($"Skipped duplicate role '{normalizedRoleName}' for group '{Name}'.");
            }
        }
    }
}
