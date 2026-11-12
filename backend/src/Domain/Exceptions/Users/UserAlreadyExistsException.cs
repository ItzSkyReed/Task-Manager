using Domain.Exceptions.Base;

namespace Domain.Exceptions.Users;

public class UserAlreadyExistsException : ConflictException
{
    public UserAlreadyExistsException(string email)
        : base($"Пользователь с Email '{email}' уже существует.", "email") { }

    public UserAlreadyExistsException()
        : base("Пользователь с таким Email уже существует.", "email") { }
}