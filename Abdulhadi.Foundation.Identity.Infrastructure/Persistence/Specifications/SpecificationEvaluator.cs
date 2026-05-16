using Microsoft.EntityFrameworkCore;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> spec) where T : class
    {
        var query = inputQuery;

        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        foreach (var include in spec.Includes)
            query = query.Include(include);

        return query;
    }
}