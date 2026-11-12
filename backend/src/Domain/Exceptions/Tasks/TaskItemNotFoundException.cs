using Domain.Exceptions.Base;

namespace Domain.Exceptions.Tasks;

public sealed class TaskItemNotFoundException(Guid taskId)
    : NotFoundException($"Задача с идентификатором '{taskId}' не найдена.");