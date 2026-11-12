using Domain.Exceptions.Base;

namespace Domain.Exceptions.Users;

public class WrongUserPasswordException(string user) : UnauthorizedException($"Неправильный пароль пользователя {user}.");