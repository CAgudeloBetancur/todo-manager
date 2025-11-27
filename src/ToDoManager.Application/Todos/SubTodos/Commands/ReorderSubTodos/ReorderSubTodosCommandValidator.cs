using FluentValidation;

namespace ToDoManager.Application.Todos.SubTodos.Commands.ReorderSubTodos;

public class ReorderSubTodosCommandValidator : AbstractValidator<ReorderSubTodosCommand>
{
	public ReorderSubTodosCommandValidator()
	{
		RuleFor(x => x.SubTodos)
			.NotNull()
			.WithMessage("SubTodos cannot be null.");
		
		RuleFor(x => x.TodoId)
			.NotNull()
			.WithMessage("TodoId cannot be null.");
	}
}