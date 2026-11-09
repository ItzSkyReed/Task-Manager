using Application.UseCases.Authentication.Dtos;
using MediatR;

namespace Application.UseCases.Authentication.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<(AccessTokenDto accessToken, string RefreshToken)>;