using ErrorOr;

using Microsoft.EntityFrameworkCore;

using Contracts.Common;

namespace Library.Database;

public static class QueryablePagingExtensions
{
  /// <summary>
  /// Reads one page of results from the database without blocking a thread.
  /// </summary>
  public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber,
    int pageSize, CancellationToken cancellationToken)
  {
    var totalCount = await source.CountAsync(cancellationToken);
    var items = await source
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync(cancellationToken);

    return new PagedList<T>(items, totalCount, pageNumber, pageSize);
  }
}
