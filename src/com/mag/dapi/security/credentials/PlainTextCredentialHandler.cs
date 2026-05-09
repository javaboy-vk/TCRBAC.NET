// ============================================================
// File Name:     PlainTextCredentialHandler.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Provides clear-text password matching for simple local demos.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using System.Text;
using log4net;

namespace com.mag.dapi.security.credentials;

/// <summary>
/// Clear-text credential handler. Useful for demos and local tools.
/// Do not use this for production credentials unless wrapped by another protected storage mechanism.
/// </summary>
public sealed class PlainTextCredentialHandler : ICredentialHandler
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(PlainTextCredentialHandler));

    public bool Matches(string suppliedPassword, string storedCredential)
    {
        var supplied = Encoding.UTF8.GetBytes(suppliedPassword ?? string.Empty);
        var stored = Encoding.UTF8.GetBytes(storedCredential ?? string.Empty);
        var matches = FixedTimeEquals(supplied, stored);
        Log.Info(matches
            ? "Plain-text credential comparison succeeded."
            : "Plain-text credential comparison failed.");
        return matches;
    }

    public string Mutate(string clearTextPassword)
    {
        Log.Info("Plain-text credential mutation returned the original credential text.");
        return clearTextPassword ?? string.Empty;
    }

    private static bool FixedTimeEquals(byte[] left, byte[] right)
    {
        var diff = left.Length ^ right.Length;
        var length = Math.Min(left.Length, right.Length);

        for (var i = 0; i < length; i++)
        {
            diff |= left[i] ^ right[i];
        }

        return diff == 0;
    }
}
