// ============================================================
// File Name:     RbacAuthorizer.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Enforces role-based authorization checks.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using com.mag.dapi.security.realms;
using log4net;

namespace com.mag.dapi.security.rbac;

/// <summary>
/// Central RBAC enforcement helper. This corresponds to the practical role-checking portion
/// of Tomcat's RealmBase/AuthenticatorBase flow, simplified for application code.
/// </summary>
public static class RbacAuthorizer
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(RbacAuthorizer));

    /// <summary>
    /// Returns true if the principal has at least one of the requested roles.
    /// </summary>
    public static bool HasAnyRole(Principal? principal, params string[] requiredRoles)
    {
        if (principal == null)
        {
            Log.Info("HasAnyRole denied access because no principal was supplied.");
            return false;
        }

        if (requiredRoles == null || requiredRoles.Length == 0)
        {
            Log.Info($"HasAnyRole allowed principal '{principal.Name}' because no specific roles were required.");
            return true;
        }

        var allowed = requiredRoles.Any(principal.IsInRole);
        Log.Info(allowed
            ? $"HasAnyRole allowed principal '{principal.Name}'."
            : $"HasAnyRole denied principal '{principal.Name}'.");
        return allowed;
    }

    /// <summary>
    /// Returns true if the principal has every requested role.
    /// </summary>
    public static bool HasAllRoles(Principal? principal, params string[] requiredRoles)
    {
        if (principal == null)
        {
            Log.Info("HasAllRoles denied access because no principal was supplied.");
            return false;
        }

        if (requiredRoles == null || requiredRoles.Length == 0)
        {
            Log.Info($"HasAllRoles allowed principal '{principal.Name}' because no specific roles were required.");
            return true;
        }

        var allowed = requiredRoles.All(principal.IsInRole);
        Log.Info(allowed
            ? $"HasAllRoles allowed principal '{principal.Name}'."
            : $"HasAllRoles denied principal '{principal.Name}'.");
        return allowed;
    }

    /// <summary>
    /// Applies the first matching security constraint. If no constraint matches, access is allowed.
    /// If a matching constraint has no roles, an authenticated user is sufficient.
    /// </summary>
    public static bool IsAllowed(Principal? principal, string path, string? method, IEnumerable<SecurityConstraint> constraints)
    {
        foreach (var constraint in constraints ?? Enumerable.Empty<SecurityConstraint>())
        {
            if (!constraint.Matches(path, method)) continue;
            if (principal == null)
            {
                Log.Info($"Authorization denied for path '{path}' because a matching constraint requires authentication.");
                return false;
            }

            if (constraint.RequiredRoles.Count == 0)
            {
                Log.Info($"Authorization allowed principal '{principal.Name}' for path '{path}' because authentication is sufficient.");
                return true;
            }

            var allowed = constraint.RequiredRoles.Any(principal.IsInRole);
            Log.Info(allowed
                ? $"Authorization allowed principal '{principal.Name}' for path '{path}'."
                : $"Authorization denied principal '{principal.Name}' for path '{path}'.");
            return allowed;
        }

        Log.Info($"Authorization allowed path '{path}' because no security constraint matched.");
        return true;
    }
}
