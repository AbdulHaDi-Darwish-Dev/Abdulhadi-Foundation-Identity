using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Specifications;

using System.Linq.Expressions;

public abstract class Specification<T> : ISpecification<T> where T : class
{
    public Expression<Func<T, bool>>? Criteria { get; protected set; }

    public List<Expression<Func<T, object>>> Includes { get; } = new();
}