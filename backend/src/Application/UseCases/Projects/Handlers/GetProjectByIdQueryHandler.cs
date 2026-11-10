using Application.Interfaces;
using Application.UseCases.Projects.Dtos;
using Application.UseCases.Projects.Mappers;
using Application.UseCases.Projects.Queries;
using Domain.Exceptions.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Projects.Handlers;

public sealed class GetProjectByIdQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var projectDto = await dbContext.Projects
                             .AsNoTracking()
                             .Where(p => p.Id == request.Id)
                             .ToProjectDto()
                             .FirstOrDefaultAsync(cancellationToken)
                         ?? throw new ProjectNotFoundException(request.Id);
        return projectDto;
    }
}