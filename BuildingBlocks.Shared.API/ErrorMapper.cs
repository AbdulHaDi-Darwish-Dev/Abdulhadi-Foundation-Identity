using Microsoft.AspNetCore.Http;
using BuildingBlocks.Shared.Core;

namespace BuildingBlocks.Shared.API
{
    public static class ErrorMapper
    {
        public static int MapToStatusCode(string? errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
                return StatusCodes.Status500InternalServerError;

            return errorCode switch
            {
                // 🌍 General / Shared
                ErrorCodes.Conflict => StatusCodes.Status409Conflict,
                ErrorCodes.NotFound => StatusCodes.Status404NotFound,
                ErrorCodes.TooManyRequests => StatusCodes.Status429TooManyRequests,

                // 🔐 Authentication / Authorization
                ErrorCodes.VerificationFailed => StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidCredentials => StatusCodes.Status401Unauthorized,
                ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorCodes.TokenExpired => StatusCodes.Status401Unauthorized,
                ErrorCodes.InvalidToken => StatusCodes.Status401Unauthorized,
                ErrorCodes.TokenRevoked => StatusCodes.Status401Unauthorized,
                ErrorCodes.Forbidden => StatusCodes.Status403Forbidden,

                // 📦 Resource State
                ErrorCodes.TokenNotFound => StatusCodes.Status404NotFound,
                ErrorCodes.InactiveResource => StatusCodes.Status403Forbidden,

                // 🛡 Permissions
                ErrorCodes.PermissionDenied => StatusCodes.Status403Forbidden,
                ErrorCodes.PermissionNotFound => StatusCodes.Status404NotFound,

                // 📦 Validation
                ErrorCodes.ValidationError => StatusCodes.Status400BadRequest,
                ErrorCodes.InvalidInput => StatusCodes.Status400BadRequest,

                // 💾 Persistence
                ErrorCodes.DatabaseError => StatusCodes.Status500InternalServerError,
                ErrorCodes.ConcurrencyError => StatusCodes.Status409Conflict,

                // ⚠️ Fallback
                ErrorCodes.UnexpectedError => StatusCodes.Status500InternalServerError,

                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}