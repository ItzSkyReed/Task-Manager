
using Domain.Enums;

namespace Application.UseCases.Tasks.Dtos;

/// <summary>
/// Подробное DTO для просмотра детальной карточки и ответа команд изменения.
/// </summary>
public sealed record TaskItemDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    TaskItemStatus Status,
    Guid ProjectId,
    string ProjectName,
    Guid? AssigneeId,
    string? AssigneeName,
    string? AssigneeEmail,
    DateTimeOffset CreatedAt);