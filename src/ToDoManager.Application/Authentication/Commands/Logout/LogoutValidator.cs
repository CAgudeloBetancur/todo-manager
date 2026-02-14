using FluentValidation;

namespace ToDoManager.Application.Authentication.Commands.Logout;

public class LogoutValidator : AbstractValidator<LogoutCommand>
{
	public LogoutValidator()
	{
		RuleFor(x => x.RefreshToken)
			.NotEmpty()
			.WithMessage("Refresh token is required");
	}
}