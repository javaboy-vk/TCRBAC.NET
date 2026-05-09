// ============================================================
// File Name:     Role.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Represents a Tomcat-style security role.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using log4net;

namespace com.mag.dapi.security.model;

/// <summary>
/// Represents a named security role, equivalent in spirit to Tomcat's Role model.
/// Roles are the atomic RBAC permissions used by the authorizer.
/// </summary>
public sealed class Role
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Role));

    /// <summary>
    /// Creates a role with a required role name.
    /// </summary>
    public Role(string name, string? description = null)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Role name is required.", nameof(name))
            : name.Trim();
        Description = description;
        Log.Info($"Created role '{Name}'.");
    }

    /// <summary>
    /// The role identifier, for example: manager-gui, admin-gui, dapi-admin.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Optional human-readable explanation of what the role allows.
    /// </summary>
    public string? Description { get; }

    public override string ToString() => Name;
}
