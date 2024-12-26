using Contracts.Courses.Requests;
using Contracts.Courses.Responses;
using Library.Pagination;

namespace Aspire_App.Web.Services.Courses;

public interface ICoursesApiService
{
    Task<PagedList<CourseResponse>> GetCoursesListAsync(int page, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<CourseResponse> GetCourse(Guid guid, CancellationToken cancellationToken = default);
    Task EnrollToCourse(Guid guid, CancellationToken cancellationToken = default);
    Task LeaveCourse(Guid guid, CancellationToken cancellationToken = default);
    Task CreateCourse(CreateCourseRequest guid, CancellationToken cancellationToken = default);

    Task LeaveCourseByAdmin(ChangeCourseEnrollmentAdminRequest adminRequest, CancellationToken cancellationToken = default);

    Task EnrollToCourseByAdmin(ChangeCourseEnrollmentAdminRequest adminRequest,
        CancellationToken cancellationToken = default);
}