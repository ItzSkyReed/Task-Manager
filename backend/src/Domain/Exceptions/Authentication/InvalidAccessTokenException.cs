using Domain.Exceptions.Base;

namespace Domain.Exceptions.Authentication;

public class InvalidAccessTokenException() : UnauthorizedException("Недействительный или просроченный Access Token.");