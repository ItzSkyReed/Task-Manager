using Application.UseCases.Authentication.Dtos;
using MediatR;

namespace Application.UseCases.Authentication.Commands;

public record LoginCommand(string Email, string Password) : IRequest<(AuthResponseDto Body, string RefreshToken)>;