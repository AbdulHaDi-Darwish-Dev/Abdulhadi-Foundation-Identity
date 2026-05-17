using System.Linq.Expressions;
using Abdulhadi.Foundation.Identity.Domain.Common;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

public interface ISpecification<T> where T : class
{
    bool IgnoreQueryFilters { get; }

    Expression<Func<T, bool>>? Criteria { get; }

    List<Expression<Func<T, object>>> Includes { get; }
}