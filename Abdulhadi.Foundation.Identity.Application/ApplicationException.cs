using BuildingBlocks.Shared.Core;

namespace abdulhadi.foundation.identity.application;

public sealed class ApplicationException : AppException
{
    public ApplicationException(string message, string? errorCode = null)
        : base(message, errorCode, "Abdulhadi.Foundation.Identity.Application")
    {
    }
}