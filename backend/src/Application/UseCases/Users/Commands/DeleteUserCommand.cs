using MediatR;

namespace Application.UseCases.Users.Commands;

public sealed record DeleteUserCommand(Guid UserId) : IRequest;