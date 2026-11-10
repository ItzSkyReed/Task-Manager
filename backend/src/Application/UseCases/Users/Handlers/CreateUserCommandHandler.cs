using Application.Interfaces;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Mappers;
using Domain.Entities;
using Domain.Exceptions.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Handlers;

/// <summary>
/// Обработчик команды создания нового пользователя для вкладки "Пользователи", не регистрации!
/// </summary>
public sealed class CreateUserCommandHandler(
    IAppDbContext dbContext,
    IPasswordHasher passwordHasher)
    : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailExists)
            throw new UserAlreadyExistsException(request.Email);

        var hashedPassword = passwordHasher.Hash(request.Password);
        var user = new User(normalizedEmail, hashedPassword, request.Name);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.ToUserDto();
    }
}