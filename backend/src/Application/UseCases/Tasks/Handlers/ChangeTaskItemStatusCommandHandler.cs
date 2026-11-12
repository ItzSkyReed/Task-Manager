using Application.Interfaces;
using Application.UseCases.Tasks.Commands;
using Application.UseCases.Tasks.Dtos;
using Application.UseCases.Tasks.Mappers;
using Domain.Exceptions.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Tasks.Handlers;

public sealed class ChangeTaskItemStatusCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<ChangeTaskItemStatusCommand, TaskItemDetailsDto>
{
    public async Task<TaskItemDetailsDto> Handle(
        ChangeTaskItemStatusCommand request,
        CancellationToken cancellationToken)
    {
        var taskItem = await dbContext.TaskItems
                           .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
                       ?? throw new TaskItemNotFoundException(request.Id);

        taskItem.ChangeStatus(request.Status);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.TaskItems
            .AsNoTracking()
            .Where(t => t.Id == taskItem.Id)
            .ToDetailsDto()
            .FirstAsync(cancellationToken);
    }
}