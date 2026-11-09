using Domain.Interfaces;
using JetBrains.Annotations;

namespace Domain.Entities;

/// <summary>
/// Refresh Token для сессий авторизации.
/// </summary>
public sealed class RefreshToken : BaseEntity
{
    [UsedImplicitly]
    private RefreshToken()
    {
    } // Для EF Core

    /// <summary>
    /// Инициализирует новый экземпляр токена обновления.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя-владельца.</param>
    /// <param name="plainToken">Токен (будет захэширована).</param>
    /// <param name="expiresAt">Время истечения срока действия токена по UTC.</param>
    /// <param name="tokenHasher">Сервис хэширования токенов.</param>
    /// <exception cref="ArgumentException">Выбрасывается при некорректных входных параметрах.</exception>
    public RefreshToken(Guid userId, string plainToken, DateTimeOffset expiresAt, ITokenHasher tokenHasher)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId не может быть пустым Guid", nameof(userId));

        if (string.IsNullOrWhiteSpace(plainToken))
            throw new ArgumentException("Токен не может быть пустым", nameof(plainToken));

        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Срок действия токена должен быть в будущем", nameof(expiresAt));

        UserId = userId;
        ExpiresAt = expiresAt;
        Token = tokenHasher.Hash(plainToken);
    }

    /// <summary>
    /// Id пользователя, которому принадлежит токен.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Хэш RefreshToken'a.
    /// </summary>
    public string Token { get; init; } = null!;

    /// <summary>
    /// Дата, время окончания срока действия токена по UTC.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Проверяет, действителен ли токен на текущий момент.
    /// </summary>
    /// <param name="plainToken">Токен.</param>
    /// <param name="tokenHasher">Сервис верификации хэша токенов.</param>
    public bool IsValid(string plainToken, ITokenHasher tokenHasher)
    {
        return DateTimeOffset.UtcNow < ExpiresAt && tokenHasher.Verify(plainToken, Token);
    }
}