// ============================================================
// File Name:     UserDatabaseRealm.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Authenticates users against the memory user database.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using com.mag.dapi.security.credentials;
using com.mag.dapi.security.users;
using log4net;

namespace com.mag.dapi.security.realms;

/// <summary>
/// Realm that authenticates against <see cref="MemoryUserDatabase"/>.
/// This is the direct C# analog of the Tomcat UserDatabaseRealm pattern.
/// </summary>
public sealed class UserDatabaseRealm
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(UserDatabaseRealm));
    private readonly MemoryUserDatabase _database;
    private readonly ICredentialHandler _credentialHandler;

    public UserDatabaseRealm(MemoryUserDatabase database, ICredentialHandler credentialHandler)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _credentialHandler = credentialHandler ?? throw new ArgumentNullException(nameof(credentialHandler));
        Log.Info($"Created user database realm with credential handler '{_credentialHandler.GetType().Name}'.");
    }

    /// <summary>
    /// Authenticates a username/password pair. Returns null when authentication fails.
    /// </summary>
    public Principal? Authenticate(string username, string password)
    {
        Log.Info($"Authenticating user '{username}'.");
        var user = _database.FindUser(username);
        if (user == null)
        {
            Log.Info($"Authentication failed because user '{username}' was not found.");
            return null;
        }

        if (!_credentialHandler.Matches(password ?? string.Empty, user.PasswordCredential))
        {
            Log.Info($"Authentication failed because credentials did not match for user '{user.Username}'.");
            return null;
        }

        Log.Info($"Authentication succeeded for user '{user.Username}'.");
        return new Principal(user.Username, _database.GetEffectiveRoles(user));
    }
}
