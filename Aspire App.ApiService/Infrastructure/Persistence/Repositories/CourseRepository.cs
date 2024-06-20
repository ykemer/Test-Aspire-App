using Aspire_App.ApiService.Domain.Models;
using Aspire_App.ApiService.Domain.Persistence;

namespace Aspire_App.ApiService.Infrastructure.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Course course, CancellationToken cancellationToken)
    {
        _context.Courses.Add(course);
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Course?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context
            .Courses
            .FindAsync(id, cancellationToken);
    }

    public IQueryable<Course> GetAllAsync(CancellationToken cancellationToken)
    {
        return _context.Courses;
    }
}