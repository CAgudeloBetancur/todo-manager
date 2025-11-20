using FluentValidation;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.UpdateTodo;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
	public UpdateTodoCommandValidator()
	{
		RuleFor(x => x.Title)
			.NotEmpty().WithMessage("Title is required.")
			.MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
		
		RuleFor(x => x.Description)
			.MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
		
		RuleFor(x => x.DueDate)
			.Must(date => date > DateTime.UtcNow)
			.WithMessage("Due date must be in the future.");

		RuleFor(x => x.Priority)
			.NotEmpty().WithMessage("Priority is required.")
			.Must(priority => TodoPriority.IsValidPriority( (int)priority! ))
			.WithMessage("Priority must be a valid value.");

		RuleFor(x => x.Status)
			.NotEmpty().WithMessage("Status is required.")
			.Must(TodoStatus.IsValidStatus)
			.WithMessage("Status must be a valid value.");
	}
}