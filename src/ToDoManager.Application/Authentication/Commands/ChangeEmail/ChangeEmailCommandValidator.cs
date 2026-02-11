using FluentValidation;

namespace ToDoManager.Application.Authentication.Commands.ChangeEmail;

public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
	public ChangeEmailCommandValidator()
	{
		RuleFor(c => c.UserId)
			.NotEmpty()
			.WithMessage("User ID is required.");
		RuleFor(c => c.NewEmail)
			.NotEmpty()
			.WithMessage("New Email is required.");
	}
}