using Application.Interfaces;
using Application.UseCases.Tasks.Commands;
using Application.UseCases.Tasks.Dtos;
using Application.UseCases.Tasks.Mappers;
using Domain.Entities;
using Domain.Exceptions.Projects;
using Domain.Exceptions.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Tasks.Handlers;

public sealed class CreateTaskItemCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<CreateTaskItemCommand, TaskItemDetailsDto>
{
    public async Task<TaskItemDetailsDto> Handle(
        CreateTaskItemCommand request,
        CancellationToken cancellationToken)
    {
        var projectExists = await dbContext.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (!projectExists)
            throw new ProjectNotFoundException(request.ProjectId);

        if (request.AssigneeId.HasValue)
        {
            var assigneeExists = await dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == request.AssigneeId.Value, cancellationToken);

            if (!assigneeExists)
                throw new UserNotFoundException(request.AssigneeId.Value);
        }

        var taskItem = new TaskItem(
            request.Name,
            request.ProjectId,
            request.Description,
            request.AssigneeId,
            request.Status);

        dbContext.TaskItems.Add(taskItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.TaskItems
            .AsNoTracking()
            .Where(t => t.Id == taskItem.Id)
            .ToDetailsDto()
            .FirstAsync(cancellationToken);
    }
}