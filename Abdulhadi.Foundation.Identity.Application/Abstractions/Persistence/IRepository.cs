namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

public interface IRepository<T> where T : class
{
    Task<List<T>> ListAsync(ISpecification<T> spec);

    Task<T?> FirstOrDefaultAsync(ISpecification<T> spec);

    Task AddAsync(T entity);

    void Remove(T entity);
}