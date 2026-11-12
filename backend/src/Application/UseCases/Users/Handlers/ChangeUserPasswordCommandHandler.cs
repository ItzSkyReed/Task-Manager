using Application.Interfaces;
using Application.UseCases.Users.Commands;
using Domain.Exceptions.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Handlers;

public sealed class ChangeUserPasswordCommandHandler(
    IAppDbContext dbContext,
    IPasswordHasher passwordHasher)
{
    public async Task Handle(
        ChangeUserPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
                       .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
                   ?? throw new UserNotFoundException(command.UserId);

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            throw new WrongUserPasswordException(user.Email);

        var newPasswordHash = passwordHasher.Hash(command.NewPassword);
        user.SetPasswordHash(newPasswordHash);

        // Завершаем все активные сессии пользователя при смене пароля
        await dbContext.RefreshTokens
            .Where(rt => rt.UserId == user.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}