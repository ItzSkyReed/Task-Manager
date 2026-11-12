using API.Requests;
using Application.Common.Dtos;
using Application.UseCases.Tasks.Commands;
using Application.UseCases.Tasks.Dtos;
using Application.UseCases.Tasks.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class TaskItemEndpoints
{
    public static void MapTaskItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks").WithTags("Tasks");

        group.MapGet("/", GetTaskItemsAsync)
            .Produces<PagedResponse<TaskItemSummaryDto>>()
            .ProducesValidationProblem()
            .WithSummary("Постраничное получение списка задач")
            .WithDescription("Возвращает список задач с пагинацией и поиском");

        group.MapGet("/{id:guid}", GetTaskItemByIdAsync)
            .Produces<TaskItemDetailsDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Получение подробной информации о задаче")
            .WithDescription("Возвращает детальную карточку задачи, включая описание, проект и данные исполнителя");

        group.MapPost("/", CreateTaskItemAsync)
            .Produces<TaskItemDetailsDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithSummary("Создание новой задачи")
            .WithDescription("Создает новую задачу в указанном проекте");

        group.MapPatch("/{id:guid}", PatchTaskItemAsync)
            .Produces<TaskItemDetailsDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithSummary("Редактирование задачи")
            .WithDescription("Обновляет название, описание, проект, статус и исполнителя задачи");

        group.MapPatch("/{id:guid}/status", ChangeTaskItemStatusAsync)
            .Produces<TaskItemDetailsDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithSummary("Изменение статуса задачи")
            .WithDescription("Изменяет текущий статус выполнения задачи");

        group.MapPatch("/{id:guid}/assignee", AssignTaskItemAsync)
            .Produces<TaskItemDetailsDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Назначение исполнителя на задачу")
            .WithDescription("Назначает пользователя исполнителем задачи или снимает назначение при передаче null");

        group.MapDelete("/{id:guid}", DeleteTaskItemAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Удаление задачи")
            .WithDescription("Удаляет задачу по её идентификатору");
    }

    private static async Task<IResult> GetTaskItemsAsync(
        [AsParameters] GetTaskItemsRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var query = new GetTaskItemsQuery(
            request.Search,
            request.ProjectId,
            request.AssigneeId,
            request.Status,
            request.Page,
            request.PageSize);

        var result = await mediator.Send(query, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTaskItemByIdAsync(
        Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetTaskItemByIdQuery(id), ct);
        return Results.Ok(result);
    }
    
    private static async Task<IResult> CreateTaskItemAsync(
        [FromBody] CreateTaskItemRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new CreateTaskItemCommand(
            request.Name,
            request.ProjectId,
            request.Description,
            request.AssigneeId,
            request.Status);

        var result = await mediator.Send(command, ct);
        return Results.Created($"/api/tasks/{result.Id}", result);
    }

    private static async Task<IResult> PatchTaskItemAsync(
        Guid id,
        [FromBody] PatchTaskItemRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new PatchTaskItemCommand(
            id,
            request.Name,
            request.Description,
            request.ProjectId,
            request.AssigneeId,
            request.Status);

        var result = await mediator.Send(command, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> ChangeTaskItemStatusAsync(
        Guid id,
        [FromBody] ChangeTaskItemStatusRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new ChangeTaskItemStatusCommand(id, request.Status);
        var result = await mediator.Send(command, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> AssignTaskItemAsync(
        Guid id,
        [FromBody] AssignTaskItemRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new AssignTaskItemCommand(id, request.AssigneeId);
        var result = await mediator.Send(command, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteTaskItemAsync(
        Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        await mediator.Send(new DeleteTaskItemCommand(id), ct);
        return Results.NoContent();
    }
}