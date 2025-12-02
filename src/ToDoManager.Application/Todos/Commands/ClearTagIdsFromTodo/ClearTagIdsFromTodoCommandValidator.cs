using FluentValidation;

namespace ToDoManager.Application.Todos.Commands.ClearTagIdsFromTodo;

public class ClearTagIdsFromTodoCommandValidator : AbstractValidator<ClearTagIdsFromTodoCommand>
{
	public ClearTagIdsFromTodoCommandValidator()
	{
		RuleFor(x => x.TodoId)
			.NotNull()
			.WithMessage("Todo Id cannot be empty.");
	}
}