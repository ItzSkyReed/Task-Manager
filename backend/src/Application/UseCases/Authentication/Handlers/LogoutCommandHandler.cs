using Application.Interfaces;
using Application.UseCases.Authentication.Commands;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Authentication.Handlers;

public class LogoutCommandHandler(
    IAppDbContext context,
    ITokenHasher tokenHasher) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = tokenHasher.Hash(request.RefreshToken);
        var tokenEntity = await context.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == tokenHash, cancellationToken);

        if (tokenEntity != null)
        {
            context.RefreshTokens.Remove(tokenEntity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}