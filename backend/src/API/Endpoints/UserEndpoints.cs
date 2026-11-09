using System.Security.Claims;
using API.Requests;
using Application.UseCases.Users.Commands;
using Application.UseCases.Users.Dtos;
using Application.UseCases.Users.Queries;
using Domain.Exceptions.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", GetMeAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<UserDto>()
            .WithSummary("Получение информации о своем профиле");

        group.MapGet("/{id:guid}", GetUserByIdAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<UserDto>()
            .WithSummary("Получение информации о пользователе по UUID");

        group.MapGet("/{email}", GetUserByEmailAsync)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<UserDto>()
            .WithSummary("Получение информации о пользователе по Email");

        group.MapPatch("/me/name", UpdateUserNameAsync)
            .Produces<UserDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Изменение имени пользователя на новое");

        group.MapPatch("/me/password", ChangeUserPasswordAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Изменение пароля пользователя");

        group.MapDelete("/me", DeleteMeAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Удаление своего аккаунта");

        group.MapDelete("/{id:guid}", DeleteUserAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Удаление своего аккаунта");

    }

    private static async Task<IResult> GetMeAsync(
        [FromServices] ISender mediator,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidAccessTokenException();

        var query = new GetUserByIdQuery(userId);
        var response = await mediator.Send(query, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> GetUserByIdAsync(
        [FromRoute] Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var query = new GetUserByIdQuery(id);
        var response = await mediator.Send(query, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> GetUserByEmailAsync(
        [FromRoute] string email,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var query = new GetUserByEmailQuery(email);
        var response = await mediator.Send(query, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> UpdateUserNameAsync(
        [FromBody] UpdateUserNameRequest request,
        ClaimsPrincipal principal,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidAccessTokenException();

        var command = new UpdateUserNameCommand(userId, request.NewName);
        var response = await mediator.Send(command, ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> ChangeUserPasswordAsync(
        [FromBody] ChangeUserPasswordRequest request,
        ClaimsPrincipal principal,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidAccessTokenException();

        var command = new ChangeUserPasswordCommand(userId, request.CurrentPassword, request.NewPassword);
        await mediator.Send(command, ct);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteMeAsync(
        ClaimsPrincipal principal,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidAccessTokenException();

        var command = new DeleteUserCommand(userId);
        await mediator.Send(command, ct);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUserAsync(
        Guid id,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var command = new DeleteUserCommand(id);
        await mediator.Send(command, ct);

        return Results.NoContent();
    }
}