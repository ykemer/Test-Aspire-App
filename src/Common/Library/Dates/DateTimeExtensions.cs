namespace Library.Dates;

public static class DateTimeExtensions
{
  /// <summary>
  /// Returns the same moment in time, expressed in UTC.
  /// <para>
  /// Why this is needed: the services run Npgsql in "legacy timestamp" mode, which returns dates read from
  /// the database in the server's LOCAL time. Comparing those with a UTC "now", or sending them as UTC,
  /// is wrong by the server's time-zone offset. Call this on every date loaded from the database
  /// before comparing it or sending it out.
  /// </para>
  /// Unspecified dates are assumed to already be UTC (that is how this system stores them).
  /// </summary>
  public static DateTime AsUtc(this DateTime value) =>
    value.Kind switch
    {
      DateTimeKind.Utc => value,
      DateTimeKind.Local => value.ToUniversalTime(),
      _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
