using Contracts.Common;
using Contracts.Students.Requests;
using Contracts.Students.Responses;

using Platform.Features.Students.GetStudent;

using StudentsGRPCClient;

namespace Platform.Features.Students.ListStudents;

public static class ListStudentsMapper
{
  public static GrpcListStudentsRequest ToGrpcListStudentsRequest(this ListStudentsRequest request) =>
    new() { Page = request.PageNumber, PageSize = request.PageSize };

  public static PagedList<StudentResponse> ToStudentListResponse(this GrpcListStudentsResponse response) =>
    new()
    {
      Items = response.Items.Select(s => s.ToStudentResponse()).ToList(),
      CurrentPage = response.CurrentPage,
      TotalPages = response.TotalPages,
      PageSize = response.PageSize,
      TotalCount = response.TotalCount
    };
}
