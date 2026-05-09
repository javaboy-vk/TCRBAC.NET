// ============================================================
// File Name:     Principal.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Represents an authenticated user principal and its effective roles.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using log4net;

namespace com.mag.dapi.security.realms;

/// <summary>
/// Authenticated identity with effective roles. Similar in purpose to Tomcat's GenericPrincipal.
/// </summary>
public sealed class Principal
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(Principal));
    private readonly HashSet<string> _roles;

    public Principal(string name, IEnumerable<string> roles)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Principal name is required.", nameof(name)) : name.Trim();
        _roles = new HashSet<string>(roles ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        Log.Info($"Created principal '{Name}' with {_roles.Count} effective role(s).");
    }

    public string Name { get; }

    public IReadOnlyCollection<string> Roles => _roles;

    public bool IsInRole(string roleName)
    {
        var isInRole = !string.IsNullOrWhiteSpace(roleName) && _roles.Contains(roleName.Trim());
        Log.Info(isInRole
            ? $"Principal '{Name}' has role '{roleName?.Trim()}'."
            : $"Principal '{Name}' does not have role '{roleName?.Trim()}'.");
        return isInRole;
    }
}
