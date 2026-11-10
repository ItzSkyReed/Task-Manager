namespace Application.UseCases.Projects.Dtos;

public sealed record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? CreatedByUserId,
    DateTimeOffset CreatedAt,
    int TasksCount = 0);