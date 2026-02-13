using ToDoManager.Domain.Users;

namespace ToDoManager.Application.Authentication.Common;

public record AuthenticationResult(
	User User,
	string Token,
	string RefreshToken
	);