using Application.Interfaces;
using Application.UseCases.Tasks.Commands;
using Application.UseCases.Tasks.Dtos;
using Application.UseCases.Tasks.Mappers;
using Domain.Exceptions.Tasks;
using Domain.Exceptions.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Tasks.Handlers;

public sealed class AssignTaskItemCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<AssignTaskItemCommand, TaskItemDetailsDto>
{
    public async Task<TaskItemDetailsDto> Handle(
        AssignTaskItemCommand request,
        CancellationToken cancellationToken)
    {
        var taskItem = await dbContext.TaskItems
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new TaskItemNotFoundException(request.Id);

        if (request.AssigneeId.HasValue)
        {
            var userExists = await dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == request.AssigneeId.Value, cancellationToken);

            if (!userExists)
                throw new UserNotFoundException(request.AssigneeId.Value);
        }

        taskItem.AssignTo(request.AssigneeId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.TaskItems
            .AsNoTracking()
            .Where(t => t.Id == taskItem.Id)
            .ToDetailsDto()
            .FirstAsync(cancellationToken);
    }
}