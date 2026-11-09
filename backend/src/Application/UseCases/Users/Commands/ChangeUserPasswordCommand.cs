namespace Application.UseCases.Users.Commands;

public sealed record ChangeUserPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword);