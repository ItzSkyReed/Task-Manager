using Application.UseCases.Projects.Dtos;
using MediatR;

namespace Application.UseCases.Projects.Commands;

public sealed record PatchProjectCommand(
    Guid Id,
    string Name,
    string? Description) : IRequest<ProjectDto>;