using System.Linq.Expressions;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

public interface ISpecification<T> where T : class
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
}