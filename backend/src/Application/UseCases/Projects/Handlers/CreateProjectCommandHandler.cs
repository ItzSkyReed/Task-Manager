using Application.Interfaces;
using Application.UseCases.Projects.Commands;
using Application.UseCases.Projects.Dtos;
using Application.UseCases.Projects.Mappers;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Projects.Handlers;

public sealed class CreateProjectCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    public async Task<ProjectDto> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = new Project(request.Name, request.Description, request.CreatedByUserId);

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        return project.ToProjectDto();
    }
}