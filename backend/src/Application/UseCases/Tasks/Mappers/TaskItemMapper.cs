using Application.UseCases.Tasks.Dtos;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.UseCases.Tasks.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TaskItemMapper
{
    public static partial TaskItemSummaryDto ToSummaryDto(this TaskItem entity);

    public static partial TaskItemDetailsDto ToDetailsDto(this TaskItem entity);


    public static partial IQueryable<TaskItemSummaryDto> ToSummaryDto(this IQueryable<TaskItem> query);

    public static partial IQueryable<TaskItemDetailsDto> ToDetailsDto(this IQueryable<TaskItem> query);
}