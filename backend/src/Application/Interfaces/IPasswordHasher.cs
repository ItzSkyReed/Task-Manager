namespace Application.Interfaces;

public interface IPasswordHasher
{
    /// <summary>
    /// Хэширует пароль
    /// </summary>
    /// <param name="password">Сырой пароль</param>
    /// <returns>Хэш пароля</returns>
    string Hash(string password);
    /// <summary>
    /// Проверяет пароль на соответствие хэшированному варианту
    /// </summary>
    /// <param name="password">Сырой пароль</param>
    /// <param name="hashedPassword"> Хэш пароля из БД</param>
    bool Verify(string password, string hashedPassword);
}