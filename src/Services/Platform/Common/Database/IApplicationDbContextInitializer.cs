namespace Platform.Common.Database;

public interface IApplicationDbContextInitializer
{
  Task InitialiseAsync();
  Task SeedAsync();
  Task TrySeedAsync();
}
