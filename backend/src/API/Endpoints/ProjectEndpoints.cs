using System.Security.Claims;
using API.Requests;
using Application.Common.Dtos;
using Application.UseCases.Projects.Commands;
using Application.UseCases.Projects.Dtos;
using Application.UseCases.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects").WithTags("Projects");

        group.MapGet("/", GetProjectsAsync)
            .Produces<PagedResponse<ProjectDto>>()
            .ProducesValidationProblem()
            .WithSummary("Постраничное получение проектов")
            .WithDescription("Возвращает список проектов с пагинацией и опциональным поиском по названию");

        group.MapGet("/{id:guid}", GetProjectByIdAsync)
            .Produces<ProjectDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Получение проекта по ID")
            .WithDescription("Возвращает подробную информацию о проекте и количество его задач");

        group.MapPost("/", CreateProjectAsync)
            .Produces<ProjectDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .WithSummary("Создание нового проекта")
            .WithDescription("Создает новый проект. Если пользователь авторизован, сохраняет его ID как автора");

        group.MapPatch("/{id:guid}", PatchProjectAsync)
            .Produces<ProjectDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithSummary("Редактирование проекта")
            .WithDescription("Обновляет название и описание существующего проекта");

        group.MapDelete("/{id:guid}", DeleteProjectAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Удаление проекта")
            .WithDescription("Удаляет проект и все привязанные к нему задачи");
    }

    private static async Task<IResult> GetProjectsAsync(
        [AsParameters] GetProjectsRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var query = new GetProjectsQuery(request.Search, request.Page, request.PageSize);
        var result = await mediator.Send(query, ct);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetProjectByIdAsync(
        Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetProjectByIdQuery(id), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateProjectAsync(
        [FromBody] CreateProjectRequest request,
        ClaimsPrincipal principal,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? createdByUserId = Guid.TryParse(userIdClaim, out var parsedId) ? parsedId : null;

        var command = new CreateProjectCommand(request.Name, request.Description, createdByUserId);
        var projectDto = await mediator.Send(command, ct);

        return Results.Created($"/api/v1/projects/{projectDto.Id}", projectDto);
    }


    private static async Task<IResult> PatchProjectAsync(
        Guid id,
        [FromBody] PatchProjectRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new PatchProjectCommand(id, request.Name, request.Description);
        var projectDto = await mediator.Send(command, ct);

        return Results.Ok(projectDto);
    }


    private static async Task<IResult> DeleteProjectAsync(
        Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        await mediator.Send(new DeleteProjectCommand(id), ct);
        return Results.NoContent();
    }
}