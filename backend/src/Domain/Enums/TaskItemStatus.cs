namespace Domain.Enums;

/// <summary>
/// Представляет жизненный цикл задачи.
/// </summary>
public enum TaskItemStatus
{
    /// <summary>
    /// Задача ожидает выполнения.
    /// </summary>
    ToDo = 0,

    /// <summary>
    /// Задача находится в работе.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Задача находится на проверке.
    /// </summary>
    OnReview = 2,

    /// <summary>
    /// Задача завершена.
    /// </summary>
    Done = 3
}