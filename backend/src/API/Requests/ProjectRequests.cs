namespace API.Requests;

public sealed record GetProjectsRequest(
    string? Search = null,
    int Page = 1,
    int PageSize = 10);

public sealed record CreateProjectRequest(
    string Name,
    string? Description = null);

public sealed record PatchProjectRequest(
    string Name,
    string? Description = null);