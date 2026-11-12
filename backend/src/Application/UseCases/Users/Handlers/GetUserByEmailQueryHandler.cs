using Application.Interfaces;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Mappers;
using Application.UseCases.Users.Queries;
using Domain.Exceptions.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Handlers;

public class GetUserByEmailQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetUserByEmailQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var userEntity = await dbContext.Users.AsNoTracking()
            .SingleOrDefaultAsync(r => r.Email == request.Email, cancellationToken);

        return userEntity is null
            ? throw new UserNotFoundException(request.Email)
            : userEntity.ToUserDto();
    }
}