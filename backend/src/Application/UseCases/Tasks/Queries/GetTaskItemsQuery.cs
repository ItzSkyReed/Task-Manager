using Application.Common.Dtos;
using Application.UseCases.Tasks.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Tasks.Queries;

/// <summary>
/// Запрос на получение постраничного списка задач
/// </summary>
public sealed record GetTaskItemsQuery(
    string? Search = null,
    Guid? ProjectId = null,
    Guid? AssigneeId = null,
    TaskItemStatus? Status = null,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResponse<TaskItemSummaryDto>>;