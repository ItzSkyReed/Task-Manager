using Domain.Exceptions.Base;

namespace Domain.Exceptions.Projects;

public sealed class ProjectNotFoundException(Guid projectId)
    : NotFoundException($"Проект с идентификатором '{projectId}' не найден.");