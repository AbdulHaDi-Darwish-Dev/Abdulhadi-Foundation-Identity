using System.Security.Claims;
using BuildingBlocks.Shared.Core;

namespace Abdulhadi.Foundation.Identity.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? throw new ApiException("User ID not found in token", ErrorCodes.Unauthorized);
    }

    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value
               ?? throw new ApiException("Role not found in token", ErrorCodes.Unauthorized);
    }
}