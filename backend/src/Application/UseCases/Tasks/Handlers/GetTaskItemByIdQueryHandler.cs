using Application.Interfaces;
using Application.UseCases.Tasks.Dtos;
using Application.UseCases.Tasks.Mappers;
using Application.UseCases.Tasks.Queries;
using Domain.Exceptions.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Tasks.Handlers;

public sealed class GetTaskItemByIdQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetTaskItemByIdQuery, TaskItemDetailsDto>
{
    public async Task<TaskItemDetailsDto> Handle(
        GetTaskItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        var taskDto = await dbContext.TaskItems
                          .AsNoTracking()
                          .Where(t => t.Id == request.Id)
                          .ToDetailsDto()
                          .FirstOrDefaultAsync(cancellationToken)
                      ?? throw new TaskItemNotFoundException(request.Id);

        return taskDto;
    }
}