using System.Data.Common;

using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Courses.Application.Setup;

public class FullTextSearchInterceptor : DbCommandInterceptor
{
  public override InterceptionResult<DbDataReader> ReaderExecuting(
    DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
  {

    if (command.CommandText.Contains("to_tsquery") || command.CommandText.Contains("ts_rank"))
    {
      command.CommandText = command.CommandText
        .Replace("to_tsquery", "LIKE")
        .Replace("ts_rank", "CHARINDEX");
    }

    return result;
  }
}
