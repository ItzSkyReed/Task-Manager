using Application.Common.Dtos;
using Application.Interfaces;
using Application.UseCases.Tasks.Dtos;
using Application.UseCases.Tasks.Mappers;
using Application.UseCases.Tasks.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Tasks.Handlers;

public sealed class GetTaskItemsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetTaskItemsQuery, PagedResponse<TaskItemSummaryDto>>
{
    public async Task<PagedResponse<TaskItemSummaryDto>> Handle(
        GetTaskItemsQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.TaskItems.AsNoTracking();

        if (request.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == request.ProjectId.Value);

        if (request.AssigneeId.HasValue)
            query = query.Where(t => t.AssigneeId == request.AssigneeId.Value);

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var pattern = $"%{request.Search.Trim()}%";
            query = query.Where(t => EF.Functions.Like(t.Name, pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToSummaryDto()
            .ToListAsync(cancellationToken);

        return new PagedResponse<TaskItemSummaryDto>(items, totalCount, request.Page, request.PageSize);
    }
}