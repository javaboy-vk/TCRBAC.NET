# API Documentation – Tomcat User RBAC Port v1.1

This documentation is generated from the included C# source structure and XML documentation comments.

## Namespace Map

| Namespace | Responsibility |
|---|---|
| `Com.Mag.Dapi.Security.Users` | XML user, group, and role model plus XML loading. |
| `Com.Mag.Dapi.Security.Credentials` | Password/credential validation contract and handlers. |
| `Com.Mag.Dapi.Security.Realms` | Authentication workflow. |
| `Com.Mag.Dapi.Security.Rbac` | Principal and role-based authorization checks. |

## Types

### `Group`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.model`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/model/Group.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `AddRole` | Adds a role to the group. Duplicate role names are ignored. |
| Property | `Name` | Group identifier. |
| Property | `Description` | Optional group description. |

### `ICredentialHandler`

- **Kind:** `interface`
- **Namespace:** `com.mag.dapi.security.credentials`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/credentials/ICredentialHandler.cs`
- **Summary:** No summary comment found.

### `MemoryUserDatabase`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.users`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/users/MemoryUserDatabase.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `Load` | Loads a Tomcat-style XML user database from disk. |
| Method/Constructor | `FromXml` | Loads a Tomcat-style XML user database from an XML string. |
| Method/Constructor | `AddRole` |  |
| Method/Constructor | `AddGroup` |  |
| Method/Constructor | `AddUser` |  |
| Method/Constructor | `FindUser` |  |
| Method/Constructor | `GetEffectiveRoles` |  |

### `MessageDigestCredentialHandler`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.credentials`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/credentials/MessageDigestCredentialHandler.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `Matches` |  |
| Method/Constructor | `Mutate` |  |
| Property | `Algorithm` |  |
| Property | `Iterations` |  |

### `NestedCredentialHandler`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.credentials`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/credentials/NestedCredentialHandler.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `Add` |  |
| Method/Constructor | `Matches` |  |
| Method/Constructor | `Mutate` |  |

### `PlainTextCredentialHandler`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.credentials`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/credentials/PlainTextCredentialHandler.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `Matches` |  |
| Method/Constructor | `Mutate` |  |

### `Principal`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.realms`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/realms/Principal.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `IsInRole` |  |
| Property | `Name` |  |

### `Unknown`

- **Kind:** `type`
- **Namespace:** `com.mag.dapi.security.rbac`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/rbac/RbacAuthorizer.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `HasAnyRole` | Returns true if the principal has at least one of the requested roles. |
| Method/Constructor | `HasAllRoles` | Returns true if the principal has every requested role. |
| Method/Constructor | `IsAllowed` | Applies the first matching security constraint. If no constraint matches, access is allowed. If a matching constraint has no roles, an authenticated user is sufficient. |

### `Role`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.model`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/model/Role.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `ToString` |  |
| Property | `Name` | The role identifier, for example: manager-gui, admin-gui, dapi-admin. |
| Property | `Description` | Optional human-readable explanation of what the role allows. |

### `SecurityConstraint`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.rbac`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/rbac/SecurityConstraint.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `AddRole` |  |
| Method/Constructor | `AddMethod` |  |
| Method/Constructor | `Matches` |  |
| Property | `PathPattern` |  |

### `User`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.model`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/model/User.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `AddRole` |  |
| Method/Constructor | `AddGroup` |  |
| Property | `Username` | Login name from the XML username attribute. |
| Property | `PasswordCredential` | Stored password value. This may be clear text, digest text, or another supported credential format. |
| Property | `FullName` | Optional display name. |

### `UserDatabaseRealm`

- **Kind:** `class`
- **Namespace:** `com.mag.dapi.security.realms`
- **Source:** `TomcatUserRbacPort_v1_0/src/com/mag/dapi/security/realms/UserDatabaseRealm.cs`
- **Summary:** No summary comment found.

| Member Type | Name | Summary |
|---|---|---|
| Method/Constructor | `Authenticate` | Authenticates a username/password pair. Returns null when authentication fails. |
