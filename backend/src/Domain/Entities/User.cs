using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Domain.Entities;

/// <summary>
/// Сущность пользователя.
/// </summary>
public sealed class User : BaseEntity
{
    private static readonly EmailAddressAttribute EmailAttribute = new();

    [UsedImplicitly]
    private User()
    {
    } // Для EF Core

    /// <summary>
    /// Создает нового пользователя системы.
    /// </summary>
    /// <param name="email">Уникальный адрес электронной почты.</param>
    /// <param name="name">Имя пользователя.</param>
    /// <param name="passwordHash">Хэш пароля.</param>
    /// <exception cref="ArgumentException">Выбрасывается при некорректном формате данных.</exception>
    public User(string email, string name, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailAttribute.IsValid(email))
            throw new ArgumentException("Неверный формат Email", nameof(email));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя не может быть пустым", nameof(name));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Хэш пароля не может быть пустым или состоять из пробелов", nameof(passwordHash));

        Email = email.Trim().ToLowerInvariant();
        Name = name.Trim();
        PasswordHash = passwordHash;
    }

    /// <summary>
    /// Уникальный адрес электронной почты (логин пользователя).
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Хэш пароля пользователя.
    /// </summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Активные RefreshToken'ы пользователя
    /// </summary>
    public IReadOnlyCollection<RefreshToken> RefreshTokens { get; init; } = new List<RefreshToken>();

    /// <summary>
    /// Задачи, за которые пользователь ответственен
    /// </summary>
    public IReadOnlyCollection<TaskItem> AssignedTasks { get; init; } = new List<TaskItem>();

    /// <summary>
    /// Обновляет имя пользователя
    /// </summary>
    /// <param name="name">Новое имя пользователя</param>
    /// <exception cref="ArgumentException">Выбрасывается, если имя пустое</exception>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя не может быть пустым", nameof(name));

        Name = name.Trim();
    }

    /// <summary>
    /// Обновляет хэш пароля пользователя
    /// </summary>
    /// <param name="passwordHash">Новый хэш пароля</param>
    /// <exception cref="ArgumentException">Выбрасывается, если хэш пустой</exception>
    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Хэш пароля не может быть пустым", nameof(passwordHash));

        PasswordHash = passwordHash;
    }
}