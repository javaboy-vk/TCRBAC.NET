# API Flow Diagram

This diagram shows the main runtime path through the public API when an application loads Tomcat-style users, authenticates a user, and checks role-based access.

<a href="/assets/api-flow-diagram.svg" target="_blank" rel="noopener" title="Open the API flow diagram">
  <img src="/assets/api-flow-diagram.svg" alt="API flow diagram">
</a>

## Related API Types

- `MemoryUserDatabase` loads and resolves users, groups, and roles.
- `UserDatabaseRealm` handles username/password authentication.
- `ICredentialHandler` implementations validate stored credentials.
- `Principal` carries the authenticated user's effective roles.
- `RbacAuthorizer` applies role checks and security constraints.
