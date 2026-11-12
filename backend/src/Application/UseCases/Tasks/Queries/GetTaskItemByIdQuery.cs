using Application.UseCases.Tasks.Dtos;
using MediatR;

namespace Application.UseCases.Tasks.Queries;

public sealed record GetTaskItemByIdQuery(Guid Id) : IRequest<TaskItemDetailsDto>;