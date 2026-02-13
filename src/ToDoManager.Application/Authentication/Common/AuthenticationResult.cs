using ToDoManager.Domain.Users;

namespace ToDoManager.Application.Authentication.Common;

public record AuthenticationResult(
	string Token,
	string RefreshToken
	);