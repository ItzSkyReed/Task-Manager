using Domain.Enums;

namespace API.Requests;

public sealed record GetTaskItemsRequest(
    string? Search = null,
    Guid? ProjectId = null,
    Guid? AssigneeId = null,
    TaskItemStatus? Status = null,
    int Page = 1,
    int PageSize = 10);

public sealed record CreateTaskItemRequest(
    string Name,
    Guid ProjectId,
    string? Description = null,
    Guid? AssigneeId = null,
    TaskItemStatus Status = TaskItemStatus.ToDo);

public sealed record PatchTaskItemRequest(
    string Name,
    string? Description,
    Guid ProjectId,
    Guid? AssigneeId,
    TaskItemStatus Status);

public sealed record ChangeTaskItemStatusRequest(TaskItemStatus Status);

public sealed record AssignTaskItemRequest(Guid? AssigneeId);