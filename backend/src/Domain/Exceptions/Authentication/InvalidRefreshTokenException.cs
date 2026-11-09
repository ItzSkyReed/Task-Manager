using Domain.Exceptions.Base;

namespace Domain.Exceptions.Authentication;

public class InvalidRefreshTokenException() : UnauthorizedException("Недействительный или просроченный Refresh Token.");