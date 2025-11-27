using FluentValidation;

namespace ToDoManager.Application.Todos.SubTodos.Commands.CreateSubTodo;

public class CreateSubTodoCommandValidator : AbstractValidator<CreateSubTodoCommand>
{
	public CreateSubTodoCommandValidator()
	{
		RuleFor(x => x.TodoId)
			.NotEmpty();
		
		RuleFor(x => x.Title)
			.NotEmpty()
			.WithMessage("Title is required")
			.MaximumLength(100);
		
		RuleFor(x => x.Description)
			.NotEmpty()
			.WithMessage("Description is required")
			.MaximumLength(180);

		RuleFor(x => x.IsComplete)
			.NotNull();
	}
}