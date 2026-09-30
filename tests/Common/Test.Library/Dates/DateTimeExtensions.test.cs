using Library.Dates;

namespace Test.Library.Dates;

[TestFixture]
public class DateTimeExtensionsTests
{
  [Test]
  public void UtcDate_StaysTheSame()
  {
    var utc = new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    Assert.That(utc.AsUtc(), Is.EqualTo(utc));
    Assert.That(utc.AsUtc().Kind, Is.EqualTo(DateTimeKind.Utc));
  }

  [Test]
  public void LocalDate_IsConvertedToTheSameMomentInUtc()
  {
    var utc = new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    var local = utc.ToLocalTime();

    Assert.That(local.AsUtc(), Is.EqualTo(utc));
    Assert.That(local.AsUtc().Kind, Is.EqualTo(DateTimeKind.Utc));
  }

  [Test]
  public void UnspecifiedDate_IsTreatedAsUtc()
  {
    var unspecified = new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);

    Assert.That(unspecified.AsUtc(), Is.EqualTo(new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc)));
    Assert.That(unspecified.AsUtc().Kind, Is.EqualTo(DateTimeKind.Utc));
  }
}
