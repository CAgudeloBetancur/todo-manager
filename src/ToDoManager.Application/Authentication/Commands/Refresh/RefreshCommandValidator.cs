using FluentValidation;

namespace ToDoManager.Application.Authentication.Commands.Refresh;

public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
	public RefreshCommandValidator()
	{
		RuleFor(x => x.RefreshToken)
			.NotEmpty()
			.MaximumLength(200)
			.WithMessage("Refresh token is required");
	}
}