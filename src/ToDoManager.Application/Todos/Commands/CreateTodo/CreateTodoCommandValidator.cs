using FluentValidation;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.CreateTodo;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
	public CreateTodoCommandValidator()
	{
		RuleFor(x => x.Title)
			.NotEmpty().WithMessage("Title is required.")
			.MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

		RuleFor(x => x.Description)
			.MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
		
		RuleFor(x => x.DueDate)
			.Must(date => date == null || date > DateTime.UtcNow)
			.WithMessage("Due date must be in the future.");

		RuleFor(x => x.Priority)
			.Must(priority => priority == null || TodoPriority.IsValidPriority( (int)priority! ))
			.WithMessage("Priority must be a valid value.");

		RuleFor(x => x.Status)
			.Must(status => status == null || TodoStatus.IsValidStatus(status))
			.WithMessage("Status must be a valid value.");

	}
}