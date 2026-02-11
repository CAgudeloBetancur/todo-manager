using FluentValidation;

namespace ToDoManager.Application.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
	public ChangePasswordCommandValidator()
	{
		RuleFor(c => c.NewPassword)
			.NotEmpty()
			.WithMessage("New password is required.");
		RuleFor(c => c.CurrentPassword)
			.NotEmpty()
			.WithMessage("Current password is required.");
	}
}