using Application.UseCases.Projects.Dtos;
using MediatR;

namespace Application.UseCases.Projects.Queries;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;