using Application.Common.Dtos;
using Application.UseCases.Users.Dtos;
using MediatR;

namespace Application.UseCases.Users.Queries;

public sealed record GetUsersQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResponse<UserDto>>;