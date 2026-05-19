namespace BuildingBlocks.Shared.Core
{
    public static class ErrorCodes
    {
        // 🌍 General / Cross-Module
        public const string Conflict = "CONFLICT";
        public const string NotFound = "NOT_FOUND";
        public const string TooManyRequests = "TOO_MANY_REQUESTS";

        // 🔐 Authentication / Authorization
        public const string VerificationFailed = "VERIFICATION_FAILED";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string TokenExpired = "TOKEN_EXPIRED";
        public const string Forbidden = "FORBIDDEN";

        // 🔁 Token / Session
        public const string InvalidToken = "INVALID_TOKEN";
        public const string TokenRevoked = "TOKEN_REVOKED";
        public const string TokenNotFound = "TOKEN_NOT_FOUND";


        // 📦 Resource State
        public const string InactiveResource = "INACTIVE_RESOURCE";

        // 🛡 Permissions
        public const string PermissionDenied = "PERMISSION_DENIED";
        public const string PermissionNotFound = "PERMISSION_NOT_FOUND";

        // 📦 Validation
        public const string ValidationError = "VALIDATION_ERROR";
        public const string InvalidInput = "INVALID_INPUT";

        // 💾 Persistence / Database
        public const string DatabaseError = "DATABASE_ERROR";
        public const string ConcurrencyError = "CONCURRENCY_ERROR";

        // ⚠️ General
        public const string UnexpectedError = "UNEXPECTED_ERROR";
    }
}