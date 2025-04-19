using System;
using System.Threading;
using System.Threading.Tasks;
using Lunafy.Core.Infrastructure.Dependencies;
using Lunafy.Data.Exceptions;

namespace Lunafy.Data;

[ScopeDependency(typeof(ITransactionManager))]
public class TransactionManager : ITransactionManager
{
    private readonly LunafyDbContext _dbContext;

    public TransactionManager(LunafyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Func<Task> dbOperation, CancellationToken cancellationToken = default)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await dbOperation();
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new DatabaseTransactionFailedException(ex);
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> dbOperation, CancellationToken cancellationToken = default)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await dbOperation();
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new DatabaseTransactionFailedException(ex);
        }
    }
}