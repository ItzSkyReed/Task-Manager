using MediatR;

namespace Application.UseCases.Projects.Commands;

public sealed record DeleteProjectCommand(Guid Id) : IRequest;