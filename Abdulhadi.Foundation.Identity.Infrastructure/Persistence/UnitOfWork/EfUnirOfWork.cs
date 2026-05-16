using BuildingBlocks.Shared.Core;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Persistence.UnitOfWork;

public class EfUnitOfWork
{
    private readonly AppDbContext _context;

    public EfUnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OutputResult<bool>> CommitAsync()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return OutputResult<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            var inner = ex.InnerException?.Message;
            var full = ex.ToString();

            throw new PersistenceException(full);
        }
    }
}