using Application.UseCases.Tasks.Dtos;
using MediatR;

namespace Application.UseCases.Tasks.Commands;

public sealed record AssignTaskItemCommand(
    Guid Id,
    Guid? AssigneeId) : IRequest<TaskItemDetailsDto>;