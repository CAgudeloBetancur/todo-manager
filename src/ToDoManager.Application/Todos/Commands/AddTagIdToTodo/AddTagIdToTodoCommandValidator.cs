using FluentValidation;

namespace ToDoManager.Application.Todos.Commands.AddTagIdToTodo;

public class AddTagIdToTodoCommandValidator : AbstractValidator<AddTagIdToTodoCommand>
{
	public AddTagIdToTodoCommandValidator()
	{
		RuleFor(x => x.TagId)
			.NotNull()
			.WithMessage("Tag Id cannot be empty");
		
		RuleFor(x => x.TodoId)
			.NotNull()
			.WithMessage("Todo Id cannot be empty");
	}	
}