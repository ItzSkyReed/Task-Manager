namespace API.Requests;

public record RegisterRequest(string Email, string Password, string Name);
public record LoginRequest(string Email, string Password);