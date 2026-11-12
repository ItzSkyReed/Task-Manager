using Application.UseCases.Tasks.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Tasks.Commands;

/// <summary>
/// Команда комплексного редактирования задачи из формы редактирования.
/// </summary>
/// <param name="Id">Идентификатор редактируемой задачи.</param>
/// <param name="Name">Новое название задачи.</param>
/// <param name="Description">Новое описание задачи.</param>
/// <param name="ProjectId">Идентификатор проекта.</param>
/// <param name="AssigneeId">Идентификатор исполнителя (null для снятия исполнителя).</param>
/// <param name="Status">Статус выполнения задачи.</param>
public sealed record PatchTaskItemCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid ProjectId,
    Guid? AssigneeId,
    TaskItemStatus Status) : IRequest<TaskItemDetailsDto>;