using BuildingBlocks.Shared.Core;

namespace Abdulhadi.Foundation.Identity.Domain;

public sealed class DomainException : AppException
{
    public DomainException(string message, string? errorCode = null)
        : base(message, errorCode, "Abdulhadi.Foundation.Identity.Domain")
    {
    }
}