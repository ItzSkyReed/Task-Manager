using Application.Common.Authentication;
using Application.Interfaces;
using Application.UseCases.Authentication.Commands;
using Application.UseCases.Authentication.Dtos;
using Application.UseCases.Users.Mappers;
using Domain.Entities;
using Domain.Exceptions.Users;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Authentication.Handlers;

public class RegisterCommandHandler(
    IAppDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IOptions<JwtOptions> jwtOptions,
    ITokenHasher tokenHasher)
    : IRequestHandler<RegisterCommand, (AuthResponseDto Body, string RefreshToken)>
{
    public async Task<(AuthResponseDto Body, string RefreshToken)> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await dbContext.Users
            .AsNoTracking().AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            throw new UserAlreadyExistsException(request.Email);

        var hashedPassword = passwordHasher.Hash(request.Password);

        var user = new User(request.Email, hashedPassword, request.Name);
        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        var expirationDays = jwtOptions.Value.RefreshTokenExpirationDays;
        var expiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays);

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, expiresAt, tokenHasher);

        dbContext.RefreshTokens.Add(refreshTokenEntity);
        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ValueTuple<AuthResponseDto, string>(new AuthResponseDto(UserMapper.ToUserDto(user), accessToken), refreshToken);
    }
}