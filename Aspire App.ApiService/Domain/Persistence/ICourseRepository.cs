using Aspire_App.ApiService.Domain.Models;

namespace Aspire_App.ApiService.Domain.Persistence;

public interface ICourseRepository
{
    Task<int> AddAsync(Course course, CancellationToken cancellationToken);
    Task<Course?> GetAsync(Guid id, CancellationToken cancellationToken);
    IQueryable<Course> GetAllAsync(CancellationToken cancellationToken);
}