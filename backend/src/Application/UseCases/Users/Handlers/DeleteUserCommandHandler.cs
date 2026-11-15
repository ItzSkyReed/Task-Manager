using Application.Interfaces;
using Application.UseCases.Users.Commands;
using Domain.Exceptions.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Handlers;

public sealed class DeleteUserCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
                       .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
                   ?? throw new UserNotFoundException(command.UserId);

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}