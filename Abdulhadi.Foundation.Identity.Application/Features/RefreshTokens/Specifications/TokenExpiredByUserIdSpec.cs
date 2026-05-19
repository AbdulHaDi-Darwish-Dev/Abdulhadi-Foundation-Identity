using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

namespace Abdulhadi.Foundation.Identity.Application.Features.RefreshTokens.Specifications;

internal class TokenExpiredByUserIdSpec : Specification<RefreshToken>
{
    public TokenExpiredByUserIdSpec(Guid userId)
    {
        var now = DateTime.UtcNow;

        // 🟢 فترة السماح: نحذف التوكنز المسحوبة فقط إذا مر عليها أكثر من 24 ساعة
        var deleteThreshold = now.AddDays(-1);

        Criteria = r => r.UserId == userId &&
            (r.ExpiresAt <= now || (r.RevokedAt != null && r.RevokedAt <= deleteThreshold));

        AsTracking = true;
    }
}