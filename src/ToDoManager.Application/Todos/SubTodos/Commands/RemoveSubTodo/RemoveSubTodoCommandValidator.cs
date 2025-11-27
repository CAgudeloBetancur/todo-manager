using FluentValidation;

namespace ToDoManager.Application.Todos.SubTodos.Commands.RemoveSubTodo;

public class RemoveSubTodoCommandValidator : AbstractValidator<RemoveSubTodoCommand>
{
	public RemoveSubTodoCommandValidator()
	{
		RuleFor(x => x.TodoId)
			.NotEmpty()
			.WithMessage("TodoId is required.");
		
		RuleFor(x => x.SubTodoId)
			.NotEmpty()
			.WithMessage("SubTodoId is required.");
		
	}
}