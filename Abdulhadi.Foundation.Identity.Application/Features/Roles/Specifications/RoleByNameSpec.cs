using Microsoft.AspNetCore.Identity;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

namespace Abdulhadi.Foundation.Identity.Application.Features.Roles.Specifications;

public class RoleByNameSpec : Specification<IdentityRole<Guid>>
{
    public RoleByNameSpec(string roleName)
    {
        string normalizedName = roleName.Trim().ToUpper();
        Criteria = r => r.NormalizedName == normalizedName;
    }
}