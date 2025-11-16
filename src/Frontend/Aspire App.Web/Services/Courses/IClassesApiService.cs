using Contracts.Classes.Requests;
using Contracts.Common;
using Contracts.Courses.Requests;
using Contracts.Courses.Responses;
using Contracts.Enrollments.Responses;

namespace Aspire_App.Web.Services.Courses;

public interface IClassesApiService
{
  Task<PagedList<ClassListItemResponse>> GetClassListAsync(string courseId, int page, int pageSize = 10,
    CancellationToken cancellationToken = default);

  Task<ClassResponse> GetClass(Guid courseId, Guid classId, CancellationToken cancellationToken = default);
  Task<List<EnrollmentResponse>> GetClassEnrollments(Guid courseId, Guid classId, CancellationToken cancellationToken = default);

  Task CreateClass(Guid courseId, CreateClassRequest createClassRequest, CancellationToken cancellationToken = default);
  Task DeleteClass(Guid courseId, Guid classId, CancellationToken cancellationToken = default);
  Task UpdateClass(Guid courseId, Guid classId,UpdateClassRequest updateCourseRequest, CancellationToken cancellationToken = default);
}
