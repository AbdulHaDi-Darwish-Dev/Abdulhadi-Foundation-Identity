using BuildingBlocks.Shared.Core;

namespace Abdulhadi.Foundation.Identity.Infrastructure;

public sealed class InfrastructureException : AppException
{
    public InfrastructureException(string message, string? errorCode = null)
        : base(message, errorCode, "Abdulhadi.Foundation.Identity.Infrastructure")
    {
    }
}