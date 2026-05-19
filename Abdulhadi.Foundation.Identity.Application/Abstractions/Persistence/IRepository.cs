using Abdulhadi.Foundation.Identity.Domain.Common;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

public interface IRepository<T> where T : class
{
    void Remove(T entity);

    void Update(T entity);

    void RemoveRange(IEnumerable<T> entities);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

    Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
}