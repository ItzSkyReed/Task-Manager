using Application.Common.Dtos;
using Application.UseCases.Projects.Dtos;
using MediatR;

namespace Application.UseCases.Projects.Queries;

public sealed record GetProjectsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResponse<ProjectDto>>;