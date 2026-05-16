using BuildingBlocks.Shared.Core;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;

    Task<OutputResult<bool>> CommitAsync();
}