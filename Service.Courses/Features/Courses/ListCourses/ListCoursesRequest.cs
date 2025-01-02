
using Contracts.Common;
using Service.Courses.Entities;

namespace Service.Courses.Features.Courses.ListCourses;

public class ListCoursesRequest : PagedQuery, IRequest<ErrorOr<PagedList<Course>>>;