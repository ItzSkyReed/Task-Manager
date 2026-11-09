namespace Application.UseCases.Users.Commands;

public sealed record UpdateUserNameCommand(Guid UserId, string NewName);