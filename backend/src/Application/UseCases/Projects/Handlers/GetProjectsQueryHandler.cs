using Application.Common.Dtos;
using Application.Interfaces;
using Application.UseCases.Projects.Dtos;
using Application.UseCases.Projects.Mappers;
using Application.UseCases.Projects.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Projects.Handlers;

public sealed class GetProjectsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetProjectsQuery, PagedResponse<ProjectDto>>
{
    public async Task<PagedResponse<ProjectDto>> Handle(
        GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Projects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(p => EF.Functions.Like(p.Name, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToProjectDto()
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProjectDto>(items, totalCount, request.Page, request.PageSize);
    }
}