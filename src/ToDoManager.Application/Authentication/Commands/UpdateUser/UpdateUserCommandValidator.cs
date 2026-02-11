using FluentValidation;

namespace ToDoManager.Application.Authentication.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
	public UpdateUserCommandValidator()
	{
		RuleFor(x => x.UserId)
			.NotEmpty()
			.WithMessage("User Id is required.");
		RuleFor(x => x.DisplayName)
			.NotEmpty()
			.WithMessage("Display name is required.");
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
	}
}