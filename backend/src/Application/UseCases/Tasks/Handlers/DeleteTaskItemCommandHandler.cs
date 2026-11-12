using Application.Interfaces;
using Application.UseCases.Tasks.Commands;
using Domain.Exceptions.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Tasks.Handlers;

public sealed class DeleteTaskItemCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<DeleteTaskItemCommand>
{
    public async Task Handle(
        DeleteTaskItemCommand request,
        CancellationToken cancellationToken)
    {
        var taskItem = await dbContext.TaskItems
                           .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
                       ?? throw new TaskItemNotFoundException(request.Id);

        dbContext.TaskItems.Remove(taskItem);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}