namespace Aspire_App.ApiService.Persistence;

public interface IApplicationDbContextInitializer
{
    Task InitialiseAsync();
    Task SeedAsync();
    Task TrySeedAsync();
}