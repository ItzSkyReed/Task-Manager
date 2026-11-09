using JetBrains.Annotations;

namespace Domain.Entities;

/// <summary>
/// Сущность проекта, объединяющего множество задач
/// </summary>
public sealed class Project : BaseEntity
{
    [UsedImplicitly]
    private Project()
    {
    } // Для EF Core

    /// <summary>
    /// Создает новый проект
    /// </summary>
    /// <param name="name">Название проекта</param>
    /// <param name="description">Описание проекта</param>
    /// <param name="createdByUserId">Опциональный создатель проекта</param>
    /// <exception cref="ArgumentException">Выбрасывается, если название проекта пустое</exception>
    public Project(string name, string? description = null, Guid? createdByUserId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название проекта не может быть пустым", nameof(name));

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Создатель проекта не может быть пустым Guid", nameof(createdByUserId));

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CreatedByUserId = createdByUserId;
    }

    /// <summary>
    /// Название проекта
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Описание проекта
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Id создателя проекта (может быть null, если пользователь удален)
    /// </summary>
    public Guid? CreatedByUserId { get; init; }

    /// <summary>
    /// Список задач, относящихся к данному проекту
    /// </summary>
    public IReadOnlyCollection<TaskItem> Tasks { get; init; } = new List<TaskItem>();

    /// <summary>
    /// Обновляет основную информацию о проекте.
    /// </summary>
    /// <param name="name">Новое название проекта</param>
    /// <param name="description">Новое описание проекта</param>
    /// <exception cref="ArgumentException">Выбрасывается, если название пустое</exception>
    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название проекта не может быть пустым", nameof(name));

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}