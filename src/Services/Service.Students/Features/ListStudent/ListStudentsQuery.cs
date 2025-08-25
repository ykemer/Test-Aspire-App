using Contracts.Common;

using Service.Students.Database.Entities;

namespace Service.Students.Features.ListStudent;

public class ListStudentsQuery : PagedQuery, IRequest<ErrorOr<PagedList<Student>>>;
