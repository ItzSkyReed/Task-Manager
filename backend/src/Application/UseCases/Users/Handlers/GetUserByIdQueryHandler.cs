using Application.Interfaces;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Mappers;
using Application.UseCases.Users.Queries;
using Domain.Exceptions.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Users.Handlers;

public class GetUserByIdQueryHandler(
    IAppDbContext dbContext)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userEntity = await dbContext.Users.AsNoTracking().SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        return userEntity is null
            ? throw new UserNotFoundException(request.Id)
            : userEntity.ToUserDto();
    }
}