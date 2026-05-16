using Microsoft.EntityFrameworkCore;
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

    public async Task AddAsync(T entity)
        => await _context.Set<T>().AddAsync(entity);

    public void Remove(T entity)
        => _context.Set<T>().Remove(entity);

    public async Task<List<T>> ListAsync(ISpecification<T> spec)
    {
        return await SpecificationEvaluator.GetQuery(_context.Set<T>().AsQueryable(), spec).ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec)
    {
        return await SpecificationEvaluator.GetQuery(_context.Set<T>().AsQueryable(), spec).FirstOrDefaultAsync();
    }
}