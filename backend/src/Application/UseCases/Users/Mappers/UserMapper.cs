using Application.UseCases.Users.Dtos;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.UseCases.Users.Mappers;

[Mapper]
public static partial class UserMapper
{
    [MapperIgnoreSource(nameof(User.PasswordHash))]
    [MapperIgnoreSource(nameof(User.RefreshTokens))]
    [MapperIgnoreSource(nameof(User.AssignedTasks))]
    public static partial UserDto ToUserDto(this User entity);
}