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

public class LoginCommandHandler(
    IAppDbContext context,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IOptions<JwtOptions> jwtOptions,
    ITokenHasher tokenHasher)
    : IRequestHandler<LoginCommand, (AuthResponseDto Body, string RefreshToken)>
{
    public async Task<(AuthResponseDto Body, string RefreshToken)> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
            throw new UserNotFoundException(request.Email);

        var isPasswordValid = passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            throw new WrongUserPasswordException(request.Email);

        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        var expirationDays = jwtOptions.Value.RefreshTokenExpirationDays;
        var expiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays);

        var refreshTokenEntity = new RefreshToken(user.Id, refreshToken, expiresAt, tokenHasher);

        context.RefreshTokens.Add(refreshTokenEntity);
        await context.SaveChangesAsync(cancellationToken);

        return new ValueTuple<AuthResponseDto, string>(new AuthResponseDto(UserMapper.ToUserDto(user), accessToken), refreshToken);
    }
}