
namespace API.Requests;

public sealed record UpdateUserNameRequest(string NewName);

public sealed record ChangeUserPasswordRequest(string CurrentPassword, string NewPassword);

public sealed record GetUsersRequest(
    string? Search = null,
    int Page = 1,
    int PageSize = 10);

public sealed record CreateUserRequest(
    string Email,
    string Password,
    string Name);