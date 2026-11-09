using MediatR;

namespace Application.UseCases.Authentication.Commands;

public record DeleteExpiredTokensCommand : IRequest<Unit>;