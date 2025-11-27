using FluentValidation;

namespace ToDoManager.Application.Todos.SubTodos.Commands.UpdateSubTodo;

public class UpdateSubTodoCommandValidator : AbstractValidator<UpdateSubTodoCommand>
{
	public UpdateSubTodoCommandValidator()
	{
		RuleFor(x => x.TodoId)
			.NotEmpty()
			.WithMessage("The Todo Id is required");
		
		RuleFor(x => x.SubTodoId)
			.NotEmpty()
			.WithMessage("The SubTodo Id is required");

		RuleFor(x => x.Description)
			.NotEmpty()
			.WithMessage("The Description is required")
			.MaximumLength(200)
			.WithMessage("The Description must not exceed 200 characters");
		
		RuleFor(x => x.Title)
			.NotEmpty()
			.WithMessage("The Title is required")
			.MaximumLength(100)
			.WithMessage("The Title must not exceed 100 characters");
		
		RuleFor(x => x.IsComplete)
			.NotNull()
			.WithMessage("The IsComplete is required");
	}
}