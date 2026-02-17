namespace ToDoManager.Application.Authentication.Common;

public record class AuthenticationResult(string Jwt, string RefreshToken);