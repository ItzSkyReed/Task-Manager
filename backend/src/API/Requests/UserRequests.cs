
namespace API.Requests;

public sealed record UpdateUserNameRequest(string NewName);

public sealed record ChangeUserPasswordRequest(string CurrentPassword, string NewPassword);