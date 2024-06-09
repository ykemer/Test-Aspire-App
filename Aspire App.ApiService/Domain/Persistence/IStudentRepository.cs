using Aspire_App.ApiService.Domain.Models;

namespace Aspire_App.ApiService.Domain.Persistence
{
    public interface IStudentRepository
    {
        Task<int> AddAsync(Student student, CancellationToken cancellationToken);
        Task <Student?> GetAsync(Guid id, CancellationToken cancellationToken);
        IQueryable<Student> GetAllAsync(CancellationToken cancellationToken);

    }
}
