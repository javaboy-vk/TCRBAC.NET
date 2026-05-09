// ============================================================
// File Name:     SecurityConstraint.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Models a protected URL pattern and required roles.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using log4net;

namespace com.mag.dapi.security.rbac;

/// <summary>
/// Small RBAC constraint model inspired by Tomcat's SecurityConstraint/SecurityCollection pairing.
/// Pattern matching is intentionally simple for internal tooling: exact path or trailing /* prefix.
/// </summary>
public sealed class SecurityConstraint
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(SecurityConstraint));
    private readonly HashSet<string> _requiredRoles = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _methods = new(StringComparer.OrdinalIgnoreCase);

    public SecurityConstraint(string pathPattern, IEnumerable<string> requiredRoles, IEnumerable<string>? httpMethods = null)
    {
        PathPattern = string.IsNullOrWhiteSpace(pathPattern) ? throw new ArgumentException("Path pattern is required.", nameof(pathPattern)) : pathPattern.Trim();
        foreach (var role in requiredRoles ?? Enumerable.Empty<string>()) AddRole(role);
        foreach (var method in httpMethods ?? Enumerable.Empty<string>()) AddMethod(method);
        Log.Info($"Created security constraint for path pattern '{PathPattern}' with {_requiredRoles.Count} required role(s) and {_methods.Count} HTTP method constraint(s).");
    }

    public string PathPattern { get; }
    public IReadOnlyCollection<string> RequiredRoles => _requiredRoles;
    public IReadOnlyCollection<string> HttpMethods => _methods;

    public void AddRole(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            var normalizedRoleName = roleName.Trim();
            if (_requiredRoles.Add(normalizedRoleName))
            {
                Log.Info($"Added required role '{normalizedRoleName}' to constraint '{PathPattern}'.");
            }
        }
    }

    public void AddMethod(string method)
    {
        if (!string.IsNullOrWhiteSpace(method))
        {
            var normalizedMethod = method.Trim().ToUpperInvariant();
            if (_methods.Add(normalizedMethod))
            {
                Log.Info($"Added HTTP method '{normalizedMethod}' to constraint '{PathPattern}'.");
            }
        }
    }

    public bool Matches(string path, string? method = null)
    {
        if (path == null || string.IsNullOrWhiteSpace(path))
        {
            Log.Info($"Constraint '{PathPattern}' did not match because the requested path was empty.");
            return false;
        }

        if (_methods.Count > 0 && method != null && !string.IsNullOrWhiteSpace(method) && !_methods.Contains(method.Trim().ToUpperInvariant()))
        {
            Log.Info($"Constraint '{PathPattern}' did not match path '{path}' because HTTP method '{method}' is not allowed.");
            return false;
        }

        if (PathPattern == path)
        {
            Log.Info($"Constraint '{PathPattern}' matched exact path '{path}'.");
            return true;
        }

        if (PathPattern.EndsWith("/*", StringComparison.Ordinal))
        {
            var prefix = PathPattern.Substring(0, PathPattern.Length - 1);
            var prefixMatch = path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            Log.Info(prefixMatch
                ? $"Constraint '{PathPattern}' matched path '{path}' by prefix."
                : $"Constraint '{PathPattern}' did not match path '{path}'.");
            return prefixMatch;
        }

        Log.Info($"Constraint '{PathPattern}' did not match path '{path}'.");
        return false;
    }
}
