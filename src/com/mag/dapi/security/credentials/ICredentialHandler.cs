// ============================================================
// File Name:     ICredentialHandler.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Defines password verification behavior.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

namespace com.mag.dapi.security.credentials;

/// <summary>
/// C# equivalent of Tomcat's CredentialHandler concept.
/// Implementations compare a user-supplied password with the stored credential.
/// </summary>
public interface ICredentialHandler
{
    /// <summary>
    /// Returns true when the supplied password matches the stored credential.
    /// </summary>
    bool Matches(string suppliedPassword, string storedCredential);

    /// <summary>
    /// Converts a clear-text password into the storage format used by this handler.
    /// </summary>
    string Mutate(string clearTextPassword);
}
