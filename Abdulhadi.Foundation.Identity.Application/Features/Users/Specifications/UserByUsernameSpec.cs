using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

namespace Abdulhadi.Foundation.Identity.Application.Features.Users.Specifications;

public class UserByUsernameSpec : Specification<ApplicationUser>
{
    public UserByUsernameSpec(string username, bool includeDeleted = false)
    {
        Criteria = u => u.UserName == username;

        if (includeDeleted)
        {
            EnableIgnoreQueryFilters(); // 👈 استدعاء الدالة المساعدة لإخبار الـ EF بإلغاء الفلتر
        }
    }
}