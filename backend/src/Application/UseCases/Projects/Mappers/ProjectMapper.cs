using Application.UseCases.Projects.Dtos;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.UseCases.Projects.Mappers;


[Mapper]
public static partial class ProjectMapper
{
    public static partial ProjectDto ToProjectDto(this Project entity);

    public static partial IQueryable<ProjectDto> ToProjectDto(this IQueryable<Project> query);
}