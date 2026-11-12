using Application.Interfaces;
using Application.UseCases.Tasks.Commands;
using Application.UseCases.Tasks.Dtos;
using Application.UseCases.Tasks.Mappers;
using Domain.Exceptions.Projects;
using Domain.Exceptions.Tasks;
using Domain.Exceptions.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Tasks.Handlers;

public sealed class PatchTaskItemCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<PatchTaskItemCommand, TaskItemDetailsDto>
{
    public async Task<TaskItemDetailsDto> Handle(
        PatchTaskItemCommand request,
        CancellationToken cancellationToken)
    {
        var taskItem = await dbContext.TaskItems
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new TaskItemNotFoundException(request.Id);

        if (taskItem.ProjectId != request.ProjectId)
        {
            var projectExists = await dbContext.Projects
                .AsNoTracking()
                .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (!projectExists)
                throw new ProjectNotFoundException(request.ProjectId);
        }

        if (request.AssigneeId.HasValue && taskItem.AssigneeId != request.AssigneeId)
        {
            var userExists = await dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == request.AssigneeId.Value, cancellationToken);

            if (!userExists)
                throw new UserNotFoundException(request.AssigneeId.Value);
        }

        taskItem.Update(request.Name, request.Description);
        taskItem.MoveToProject(request.ProjectId);
        taskItem.AssignTo(request.AssigneeId);
        taskItem.ChangeStatus(request.Status);

        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.TaskItems
            .AsNoTracking()
            .Where(t => t.Id == taskItem.Id)
            .ToDetailsDto()
            .FirstAsync(cancellationToken);
    }
}