using MediatR;

namespace Application.UseCases.Tasks.Commands;

public sealed record DeleteTaskItemCommand(Guid Id) : IRequest;