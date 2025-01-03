using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;
using Contracts.Enrollments.Responses;

namespace Aspire_App.Web.Services.Courses;

public interface ICoursesApiService
{
    Task<PagedList<CourseListItemResponse>> GetCoursesListAsync(int page, int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<CourseResponse> GetCourse(Guid guid, CancellationToken cancellationToken = default);
    Task<List<EnrollmentResponse>> GetCourseEnrollments(Guid guid, CancellationToken cancellationToken = default);
    Task EnrollToCourse(Guid courseId, CancellationToken cancellationToken = default);
    Task LeaveCourse(Guid courseId, CancellationToken cancellationToken = default);
    Task CreateCourse(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken = default);

    Task LeaveCourseByAdmin(ChangeCourseEnrollmentAdminRequest adminRequest, CancellationToken cancellationToken = default);

    Task EnrollToCourseByAdmin(ChangeCourseEnrollmentAdminRequest adminRequest,
        CancellationToken cancellationToken = default);
}