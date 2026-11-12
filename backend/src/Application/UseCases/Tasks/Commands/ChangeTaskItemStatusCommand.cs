using Application.UseCases.Tasks.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Tasks.Commands;

public sealed record ChangeTaskItemStatusCommand(
    Guid Id,
    TaskItemStatus Status) : IRequest<TaskItemDetailsDto>;