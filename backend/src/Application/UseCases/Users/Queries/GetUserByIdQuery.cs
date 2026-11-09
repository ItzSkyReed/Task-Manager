using Application.UseCases.Users.Dtos;
using MediatR;

namespace Application.UseCases.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;