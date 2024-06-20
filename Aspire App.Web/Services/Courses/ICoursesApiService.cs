using Aspire_App.Web.Contracts.Requests.Courses;
using Aspire_App.Web.Models.Courses;
using Library.Pagination;

namespace Aspire_App.Web.Services.Courses;

public interface ICoursesApiService
{
    Task<PagedList<CourseResponse>> GetCoursesListForStudentAsync(int page, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<PagedList<CourseResponse>> GetCoursesListForAdminAsync(int page, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<CourseResponse> GetCourse(Guid guid, CancellationToken cancellationToken = default);
    Task EnrollToCourse(Guid guid, CancellationToken cancellationToken = default);
    Task LeaveCourse(Guid guid, CancellationToken cancellationToken = default);
    Task CreateCourse(CreateCourseRequest guid, CancellationToken cancellationToken = default);

    Task LeaveCourseByAdmin(ChangeEnrollmentForTheCourseByAdmin request, CancellationToken cancellationToken = default);

    Task EnrollToCourseByAdmin(ChangeEnrollmentForTheCourseByAdmin request,
        CancellationToken cancellationToken = default);
}