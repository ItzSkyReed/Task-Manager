using Application.Common.Dtos;
using Application.Interfaces;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Mappers;
using Application.UseCases.Users.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Handlers;

public sealed class GetUsersQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetUsersQuery, PagedResponse<UserDto>>
{
    public async Task<PagedResponse<UserDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim();
            var pattern = $"%{searchTerm}%";

            query = query.Where(u => EF.Functions.Like(u.Name, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.Name)
            .ThenByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToUserDto()
            .ToListAsync(cancellationToken);

        return new PagedResponse<UserDto>(users, totalCount, request.Page, request.PageSize);
    }
}