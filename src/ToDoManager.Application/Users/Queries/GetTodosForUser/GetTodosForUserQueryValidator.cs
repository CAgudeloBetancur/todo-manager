using FluentValidation;

namespace ToDoManager.Application.Users.Queries.GetTodosForUser;

public class GetTodosForUserQueryValidator : AbstractValidator<GetTodosForUserQuery>
{
	public GetTodosForUserQueryValidator()
	{
		RuleFor(x => x.UserId)
			.NotEmpty()
			.WithMessage("You must provide a user ID");
	}
}