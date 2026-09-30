using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Library.Database;

public static class DbContextTransactionExtensions
{
  /// <summary>
  /// Runs <paramref name="operation"/> inside a new transaction through the context's execution strategy.
  /// Aspire turns on retries for PostgreSQL, and a retrying strategy refuses a transaction started by hand:
  /// the whole transaction must be one unit it can run again after a lost connection.
  /// <para>
  /// The operation calls <see cref="IDbContextTransaction.CommitAsync"/> itself; returning without it rolls
  /// everything back. Before a retry the change tracker is cleared, so entities added by the failed attempt are
  /// not saved twice.
  /// </para>
  /// </summary>
  public static Task<TResult> InTransactionAsync<TResult>(this DbContext dbContext,
    Func<IDbContextTransaction, CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken)
  {
    var attempt = 0;
    var strategy = dbContext.Database.CreateExecutionStrategy();

    return strategy.ExecuteAsync(async token =>
    {
      if (attempt++ > 0)
      {
        dbContext.ChangeTracker.Clear();
      }

      await using var transaction = await dbContext.Database.BeginTransactionAsync(token);
      return await operation(transaction, token);
    }, cancellationToken);
  }
}
