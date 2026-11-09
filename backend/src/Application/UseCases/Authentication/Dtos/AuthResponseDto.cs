using Application.UseCases.Users.Dtos;

namespace Application.UseCases.Authentication.Dtos;

public record AuthResponseDto(UserDto User, string AccessToken);