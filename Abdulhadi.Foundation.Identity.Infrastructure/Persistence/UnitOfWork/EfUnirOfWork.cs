using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;
using Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Repositories;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.UnitOfWork;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private readonly Dictionary<string, object> _repositories = new(); // قاموس لحفظ الـ Repositories

    public EfUnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity).FullName;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryInstance = new Repository<TEntity>(_context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity>)_repositories[type]!;
    }

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}