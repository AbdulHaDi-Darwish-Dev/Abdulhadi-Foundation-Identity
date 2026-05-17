using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

namespace Abdulhadi.Foundation.Identity.Application.Features.Users.Specifications;

public class UserByEmailSpec : Specification<ApplicationUser>
{
    public UserByEmailSpec(string email, bool includeDeleted = false)
    {
        Criteria = u => u.Email == email;

        if (includeDeleted)
        {
            EnableIgnoreQueryFilters();
        }
    }
}