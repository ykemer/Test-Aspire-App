using Aspire_App.ApiService.Domain.Models;

namespace Aspire_App.ApiService.Domain.Persistence;

public interface IEnrollmentRepository
{
    Task<int> AddAsync(StudentEnrollment enrollment, CancellationToken cancellationToken);
    Task<StudentEnrollment?> GetAsync(Guid id, CancellationToken cancellationToken);
    IQueryable<StudentEnrollment> GetAllAsync(CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}