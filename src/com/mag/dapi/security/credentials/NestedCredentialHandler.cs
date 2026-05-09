// ============================================================
// File Name:     NestedCredentialHandler.cs
// Project:       Tomcat User/RBAC C# Port - v1.0
// Author:        vasilis
// Date:          2026-05-06
// Version:       1.0
// Description:   Tries multiple credential handlers to support credential migration.
// Source Basis:  Practical C# port inspired by Apache Tomcat main branch
//                user database, realm, credential handler, and RBAC classes.
// Notes:         This is not a byte-for-byte translation of Apache Tomcat.
//                It preserves the conceptual structure for internal use.
// ============================================================

using log4net;

namespace com.mag.dapi.security.credentials;

/// <summary>
/// Chains multiple credential handlers. This is useful while migrating from clear-text
/// demo credentials to digest credentials, or from one digest format to another.
/// </summary>
public sealed class NestedCredentialHandler : ICredentialHandler
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(NestedCredentialHandler));
    private readonly List<ICredentialHandler> _handlers = new();

    public NestedCredentialHandler(params ICredentialHandler[] handlers)
    {
        _handlers.AddRange(handlers.Where(h => h != null));
        Log.Info($"Created nested credential handler with {_handlers.Count} configured handler(s).");
    }

    public void Add(ICredentialHandler handler)
    {
        _handlers.Add(handler);
        Log.Info($"Added credential handler '{handler.GetType().Name}' to nested handler.");
    }

    public bool Matches(string suppliedPassword, string storedCredential)
    {
        foreach (var handler in _handlers)
        {
            try
            {
                Log.Info($"Trying credential handler '{handler.GetType().Name}'.");
                if (handler.Matches(suppliedPassword, storedCredential))
                {
                    Log.Info($"Credential matched with handler '{handler.GetType().Name}'.");
                    return true;
                }
            }
            catch
            {
                // Ignore handler-specific parse failures and try the next handler.
                Log.Info($"Credential handler '{handler.GetType().Name}' could not parse the stored credential; trying the next handler.");
            }
        }

        Log.Info("No nested credential handler matched the supplied credential.");
        return false;
    }

    public string Mutate(string clearTextPassword)
    {
        if (_handlers.Count == 0) throw new InvalidOperationException("No credential handlers are configured.");
        Log.Info($"Mutating credential with first nested handler '{_handlers[0].GetType().Name}'.");
        return _handlers[0].Mutate(clearTextPassword);
    }
}
