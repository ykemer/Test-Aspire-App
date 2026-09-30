using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Library.Database;

public static class DbUpdateExceptionExtensions
{
  /// <summary>
  /// True when PostgreSQL rejected the save because a unique index or primary key already has the same value.
  /// Pass <paramref name="constraintName"/> to check for one specific index only.
  /// </summary>
  public static bool IsUniqueViolation(this DbUpdateException exception, string? constraintName = null) =>
    exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } postgresError
    && (constraintName is null || postgresError.ConstraintName == constraintName);
}
