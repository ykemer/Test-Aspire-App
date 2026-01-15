using Contracts.Users.Requests;
using Contracts.Users.Responses;

namespace Aspire_App.Web.Services.UserService;

public interface IUserService
{
  Task<UserInfoResponse> GetUserInfoAsync(CancellationToken cancellationToken);
}
