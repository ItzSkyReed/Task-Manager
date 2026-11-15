using Application.Interfaces;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Mappers;
using Domain.Exceptions.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Handlers;

public sealed class UpdateUserNameCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<UpdateUserNameCommand, UserDto>
{
    public async Task<UserDto> Handle(
        UpdateUserNameCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
                       .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
                   ?? throw new UserNotFoundException(command.UserId);

        user.UpdateName(command.NewName);

        await dbContext.SaveChangesAsync(cancellationToken);

        return user.ToUserDto();
    }
}