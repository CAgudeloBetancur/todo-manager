using FluentValidation;

namespace ToDoManager.Application.Authentication.Commands.Register;

public class RegisterCommandValidator: AbstractValidator<RegisterCommand>
{
	public RegisterCommandValidator()
	{
		RuleFor(x => x.FirstName)
			.NotEmpty()
			.WithMessage("First name is required.");
		RuleFor(x => x.LastName)
			.NotEmpty()
			.WithMessage("Lastname is required.");
		RuleFor(x => x.Email)
			.NotEmpty()
			.EmailAddress()
			.WithMessage("Email name is required.");
		RuleFor(x => x.Password)
			.NotEmpty()
			.WithMessage("Password is required.");
	}
}