using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lunafy.Data;

public interface ITransactionManager
{
    Task ExecuteAsync(Func<Task> dbOperation, CancellationToken cancellationToken = default);
    Task<T> ExecuteAsync<T>(Func<Task<T>> dbOperation, CancellationToken cancellationToken = default);
}
