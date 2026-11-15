using Application.UseCases.Users.Dtos;
using MediatR;

namespace Application.UseCases.Users.Commands;

public sealed record UpdateUserNameCommand(Guid UserId, string NewName) : IRequest<UserDto>;