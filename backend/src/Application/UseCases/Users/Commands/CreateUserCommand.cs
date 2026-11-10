using Application.UseCases.Users.Dtos;
using MediatR;

namespace Application.UseCases.Users.Commands;

public sealed record CreateUserCommand(
    string Email,
    string Password,
    string Name) : IRequest<UserDto>;