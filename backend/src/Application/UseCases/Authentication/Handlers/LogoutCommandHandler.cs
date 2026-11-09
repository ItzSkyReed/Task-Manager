using Application.Interfaces;
using Application.UseCases.Authentication.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Authentication.Handlers;

public class LogoutCommandHandler(IAppDbContext context) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenEntity = await context.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (tokenEntity != null)
        {
            context.RefreshTokens.Remove(tokenEntity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}