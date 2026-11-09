using Application.UseCases.Users.Dtos;
using MediatR;

namespace Application.UseCases.Users.Queries;

public record GetUserByEmailQuery(string Email) : IRequest<UserDto>;