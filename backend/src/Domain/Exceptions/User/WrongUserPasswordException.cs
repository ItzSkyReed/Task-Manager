using Domain.Exceptions.Base;

namespace Domain.Exceptions.User;

public class WrongUserPasswordException(string user) : UnauthorizedException($"Неправильный пароль пользователя {user}.");