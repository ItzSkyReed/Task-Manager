using Application.UseCases.Projects.Dtos;
using MediatR;

namespace Application.UseCases.Projects.Commands;

public sealed record CreateProjectCommand(
    string Name,
    string? Description,
    Guid? CreatedByUserId = null) : IRequest<ProjectDto>;