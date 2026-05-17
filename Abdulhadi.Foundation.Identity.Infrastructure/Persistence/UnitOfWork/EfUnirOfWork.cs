using BuildingBlocks.Shared.Core;
using Abdulhadi.Foundation.Identity.Domain.Common;
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
        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryInstance = new Repository<TEntity>(_context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity>)_repositories[type]!;
    }

    public async Task<OutputResult<bool>> CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();

            return OutputResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            throw new PersistenceException(ex.Message);
        }
    }
}