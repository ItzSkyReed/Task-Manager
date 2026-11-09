using Application.UseCases.Authentication.Dtos;
using MediatR;

namespace Application.UseCases.Authentication.Commands;

public record RegisterCommand(string Email, string Password, string Name) : IRequest<(AuthResponseDto Body, string RefreshToken)>;