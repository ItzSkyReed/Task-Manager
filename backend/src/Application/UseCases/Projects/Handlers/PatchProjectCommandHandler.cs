using Application.Interfaces;
using Application.UseCases.Projects.Commands;
using Application.UseCases.Projects.Dtos;
using Application.UseCases.Projects.Mappers;
using Domain.Exceptions.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Projects.Handlers;

public sealed class PatchProjectCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<PatchProjectCommand, ProjectDto>
{
    public async Task<ProjectDto> Handle(
        PatchProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects
                          .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                      ?? throw new ProjectNotFoundException(request.Id);

        project.Update(request.Name, request.Description);
        await dbContext.SaveChangesAsync(cancellationToken);

        var tasksCount = await dbContext.TaskItems
            .CountAsync(t => t.ProjectId == project.Id, cancellationToken);

        return project.ToProjectDto() with {TasksCount = tasksCount};
    }
}