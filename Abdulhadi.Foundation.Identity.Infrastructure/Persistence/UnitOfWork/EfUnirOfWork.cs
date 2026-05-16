using BuildingBlocks.Shared.Core;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;
using Abdulhadi.Foundation.Identity.Infrastructure.Persistence.Repositories;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        => new Repository<TEntity>(_context);

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