using MediatR;

namespace Application.UseCases.Authentication.Commands;

public record LogoutCommand(string RefreshToken) : IRequest;