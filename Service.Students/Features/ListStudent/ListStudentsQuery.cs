using Contracts.Common;
using Service.Students.Entitites;

namespace Service.Students.Features.ListStudent;

public class ListStudentsQuery : PagedQuery, IRequest<ErrorOr<PagedList<Student>>>; 
