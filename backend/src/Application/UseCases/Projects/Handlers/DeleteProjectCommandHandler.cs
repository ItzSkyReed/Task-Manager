using Application.Interfaces;
using Application.UseCases.Projects.Commands;
using Domain.Exceptions.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Projects.Handlers;

public sealed class DeleteProjectCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await dbContext.Projects
                          .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                      ?? throw new ProjectNotFoundException(request.Id);

        dbContext.Projects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}