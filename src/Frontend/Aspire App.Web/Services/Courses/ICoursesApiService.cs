using Contracts.Classes.Requests;
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

  Task CreateCourse(CreateCourseRequest createCourseRequest, CancellationToken cancellationToken = default);
  Task DeleteCourse(Guid id, CancellationToken cancellationToken = default);
  Task UpdateCourse(Guid courseId, UpdateCourseRequest updateCourseRequest, CancellationToken cancellationToken = default);







}
