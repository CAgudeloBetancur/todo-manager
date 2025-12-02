using FluentValidation;

namespace ToDoManager.Application.Todos.Commands.RemoveTagIdFromTodo;

public class RemoveTagIdFromTodoCommandValidator : AbstractValidator<RemoveTagIdFromTodoCommand>
{
	public RemoveTagIdFromTodoCommandValidator()
	{
		RuleFor(x => x.TagId)
			.NotNull()
			.WithMessage("Tag Id cannot be empty");
		
		RuleFor(x => x.TodoId)
			.NotNull()
			.WithMessage("Todo Id cannot be empty");
	}
}