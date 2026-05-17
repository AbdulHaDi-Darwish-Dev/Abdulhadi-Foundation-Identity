using System.Linq.Expressions;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

public abstract class Specification<T> : ISpecification<T> where T : class
{
    public bool IgnoreQueryFilters { get; protected set; } = false; // 👈 افتراضياً لا يلغي الفلاتر

    public Expression<Func<T, bool>>? Criteria { get; protected set; }

    public List<Expression<Func<T, object>>> Includes { get; } = new();

    protected void EnableIgnoreQueryFilters()
    {
        IgnoreQueryFilters = true;
    }
}