using BuildingBlocks.Shared.Core;

namespace Abdulhadi.Foundation.Identity.Api;

public class ApiException : AppException
{
    public ApiException(string message, string? errorCode = null)
        : base(message, errorCode, "Abdulhadi.Foundation.Identity.Api")
    {
    }
}