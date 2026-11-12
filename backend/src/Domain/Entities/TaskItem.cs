using Domain.Enums;
using JetBrains.Annotations;

namespace Domain.Entities;

/// <summary>
/// Сущность отдельной задачи в рамках проекта
/// </summary>
public sealed class TaskItem : BaseEntity
{
    [UsedImplicitly]
    private TaskItem()
    {
    } // Для EF Core

    /// <summary>
    /// Создает новую задачу
    /// </summary>
    /// <param name="name">Название задачи</param>
    /// <param name="projectId">Id родительского проекта</param>
    /// <param name="description">Описание задачи</param>
    /// <param name="assigneeId">Опциональный идентификатор назначенного исполнителя.</param>
    /// <param name="status">Начальный статус задачи (по умолчанию <see cref="TaskItemStatus.ToDo"/>).</param>
    /// <exception cref="ArgumentException">Выбрасывается при некорректных ID или пустом названии.</exception>
    public TaskItem(
        string name,
        Guid projectId,
        string? description = null,
        Guid? assigneeId = null,
        TaskItemStatus status = TaskItemStatus.ToDo)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название задачи не может быть пустым", nameof(name));

        if (projectId == Guid.Empty)
            throw new ArgumentException("Id проекта не может быть пустым Guid", nameof(projectId));

        if (assigneeId == Guid.Empty)
            throw new ArgumentException("Id исполнителя не может быть пустым Guid", nameof(assigneeId));

        Name = name.Trim();
        ProjectId = projectId;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        AssigneeId = assigneeId;
        Status = status;
    }

    /// <summary>
    /// Название задачи
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Описание задачи
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Текущий статус выполнения задачи
    /// </summary>
    public TaskItemStatus Status { get; private set; } = TaskItemStatus.ToDo;

    /// <summary>
    /// Идентификатор проекта, к которому привязана задача
    /// </summary>
    public Guid ProjectId { get; private set; }

    /// <summary>
    /// Навигационное свойство родительского проекта
    /// </summary>
    public Project Project { get; private set; } = null!;

    /// <summary>
    /// Идентификатор назначенного исполнителя задачи (null, если задача не назначена)
    /// </summary>
    public Guid? AssigneeId { get; private set; }

    /// <summary>
    /// Навигационное свойство пользователя-исполнителя задачи
    /// </summary>
    public User? Assignee { get; private set; }

    /// <summary>
    /// Обновляет текстовые данные задачи.
    /// </summary>
    /// <param name="name">Новое название задачи</param>
    /// <param name="description">Новое описание задачи</param>
    /// <exception cref="ArgumentException">Выбрасывается, если название пустое</exception>
    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название задачи не может быть пустым", nameof(name));

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    /// <summary>
    /// Изменяет текущий статус выполнения задачи
    /// </summary>
    /// <param name="newStatus">Новый статус задачи.</param>
    public void ChangeStatus(TaskItemStatus newStatus)
    {
        Status = newStatus;
    }

    /// <summary>
    /// Назначает исполнителя на задачу либо снимает назначение (при передаче null).
    /// </summary>
    /// <param name="assigneeId">Идентификатор пользователя или null для снятия исполнителя</param>
    /// <exception cref="ArgumentException">Выбрасывается, если передан <see cref="Guid.Empty"/>.</exception>
    public void AssignTo(Guid? assigneeId)
    {
        if (assigneeId == Guid.Empty)
            throw new ArgumentException("Id исполнителя не может быть пустым Guid", nameof(assigneeId));

        AssigneeId = assigneeId;
    }

    /// <summary>
    /// Переносит задачу в другой проект
    /// </summary>
    /// <param name="newProjectId">Идентификатор целевого проекта</param>
    /// <exception cref="ArgumentException">Выбрасывается, если передан пустой Guid</exception>
    public void MoveToProject(Guid newProjectId)
    {
        if (newProjectId == Guid.Empty)
            throw new ArgumentException("Id проекта не может быть пустым Guid", nameof(newProjectId));

        ProjectId = newProjectId;
    }
}