using Microsoft.AspNetCore.Authorization;

namespace Shared.Security;

public static class AuthorizedRoles
{
    public static AuthorizeAttribute Host => new() { Roles = "Host" };
    public static AuthorizeAttribute Customer => new() { Roles = "Customer" };
}