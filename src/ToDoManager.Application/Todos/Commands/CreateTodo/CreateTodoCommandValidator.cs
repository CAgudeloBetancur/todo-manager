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
			.MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
			.When(x => x.Description is not null);
		
		RuleFor(x => x.DueDate)
			.Must(date => date == null || date > DateTime.UtcNow)
			.WithMessage("Due date must be in the future.");

		RuleFor(x => x.Priority)
			.NotEmpty().WithMessage("Priority is required.")
			.Must(priority => TodoPriority.IsValidPriority( (int)priority! ))
			.When(x => x.Priority is not null)
			.WithMessage("Priority must be a valid value.");

		RuleFor(x => x.Status)
			.NotEmpty().WithMessage("Status is required.")
			.Must(TodoStatus.IsValidStatus)
			.When(x => x.Status is not null)
			.WithMessage("Status must be a valid value.");

	}
}