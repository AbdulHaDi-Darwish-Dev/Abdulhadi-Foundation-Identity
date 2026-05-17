using Microsoft.EntityFrameworkCore;
using Abdulhadi.Foundation.Identity.Domain.Common;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;
using Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Specifications;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public void Remove(T entity)
        => _context.Set<T>().Remove(entity);

    public void Update(T entity)
        => _context.Set<T>().Update(entity);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _context.Set<T>().AddAsync(entity, cancellationToken);

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Set<T>().FindAsync(id, cancellationToken);

    public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        => await SpecificationEvaluator.GetQuery(_context.Set<T>(), spec).CountAsync(cancellationToken);

    public async Task<bool> ExistsAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        => await SpecificationEvaluator.GetQuery(_context.Set<T>(), spec).AnyAsync(cancellationToken);

    public async Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        => await SpecificationEvaluator.GetQuery(_context.Set<T>(), spec).ToListAsync(cancellationToken);

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        => await SpecificationEvaluator.GetQuery(_context.Set<T>(), spec).FirstOrDefaultAsync(cancellationToken);
}