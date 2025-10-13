using Contracts.Common;

using Service.Students.Common.Database.Entities;

namespace Service.Students.Features.ListStudent;

public class ListStudentsQuery : PagedQuery, IRequest<ErrorOr<PagedList<Student>>>;
