using Domain.Enums;

namespace Application.UseCases.Tasks.Dtos;

/// <summary>
/// Компактное DTO для отображения задач в таблице
/// </summary>
public sealed record TaskItemSummaryDto(
    Guid Id,
    string Name,
    TaskItemStatus Status,
    Guid ProjectId,
    string ProjectName,
    Guid? AssigneeId,
    string? AssigneeName,
    DateTimeOffset CreatedAt);