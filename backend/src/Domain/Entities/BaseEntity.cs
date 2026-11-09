namespace Domain.Entities;

/// <summary>
/// Базовая сущность, от которой наследуются все доменные модели.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Уникальный идентификатор сущности (UUIDv7 для оптимальной индексации в PostgreSQL).
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>
    /// Время момента создания записи в формате UTC.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}