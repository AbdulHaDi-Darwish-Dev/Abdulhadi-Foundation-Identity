using BuildingBlocks.Shared.Core;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence;

public sealed class PersistenceException : AppException
{
    public PersistenceException(string message, string? errorCode = null)
        : base(message, errorCode, "Persistence")
    {
    }
}