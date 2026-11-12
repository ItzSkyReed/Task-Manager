using Application.UseCases.Tasks.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Tasks.Commands;

public sealed record CreateTaskItemCommand(
    string Name,
    Guid ProjectId,
    string? Description = null,
    Guid? AssigneeId = null,
    TaskItemStatus Status = TaskItemStatus.ToDo) : IRequest<TaskItemDetailsDto>;