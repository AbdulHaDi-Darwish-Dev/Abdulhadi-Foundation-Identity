using System.Linq.Expressions;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

public interface ISpecification<T> where T : class
{
    bool AsTracking { get; }

    bool IgnoreQueryFilters { get; }

    Expression<Func<T, bool>>? Criteria { get; }

    List<Expression<Func<T, object>>> Includes { get; }
}