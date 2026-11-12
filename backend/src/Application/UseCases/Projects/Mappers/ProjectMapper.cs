using Application.UseCases.Projects.Dtos;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.UseCases.Projects.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ProjectMapper
{
    [MapProperty(nameof(Project.Tasks.Count), nameof(ProjectDto.TasksCount))] // Указывает маппить Tasks.Count в TasksCount
    public static partial ProjectDto ToProjectDto(this Project entity);

    public static partial IQueryable<ProjectDto> ToProjectDto(this IQueryable<Project> query);
}