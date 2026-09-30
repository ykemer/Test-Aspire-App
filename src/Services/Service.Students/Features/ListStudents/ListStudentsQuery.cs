using Contracts.Common;

using Service.Students.Common.Database.Entities;

namespace Service.Students.Features.ListStudents;

public sealed class ListStudentsQuery : PagedQuery, IRequest<ErrorOr<PagedList<Student>>>;
