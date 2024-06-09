using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;

namespace Aspire_App.ApiService.Infrastructure.Persistence;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Student student, CancellationToken cancellationToken)
    {
        _context.Students.Add(student);
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Student?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Students.FindAsync(id, cancellationToken);
    }

    public IQueryable<Student> GetAllAsync(CancellationToken cancellationToken)
    {
        return _context.Students;
    }
}