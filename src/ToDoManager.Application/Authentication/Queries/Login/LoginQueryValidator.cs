using FluentValidation;

namespace ToDoManager.Application.Authentication.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
	public LoginQueryValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty()
			.EmailAddress()
			.WithMessage("Email is required.");
		RuleFor(x => x.Password)
			.NotEmpty()
			.WithMessage("Password is required.");
	}
}