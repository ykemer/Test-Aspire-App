using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;

namespace Aspire_App.ApiService.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(StudentEnrollment enrollment, CancellationToken cancellationToken)
    {
        _context.StudentEnrollments.Add(enrollment);
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<StudentEnrollment?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.StudentEnrollments.FindAsync(id, cancellationToken);
    }

    public IQueryable<StudentEnrollment> GetAllAsync(CancellationToken cancellationToken)
    {
        return _context.StudentEnrollments;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var enrollment = await GetAsync(id, cancellationToken);
        if (enrollment == null) throw new ArgumentException("Student enrollment not found.");
        _context.StudentEnrollments.Remove(enrollment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}