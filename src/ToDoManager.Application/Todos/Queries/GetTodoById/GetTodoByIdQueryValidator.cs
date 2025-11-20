using FluentValidation;

namespace ToDoManager.Application.Todos.Queries.GetTodoById;

public class GetTodoByIdQueryValidator : AbstractValidator<GetTodoByIdQuery>
{
	public GetTodoByIdQueryValidator()
	{
		RuleFor(x => x.TodoId).NotNull();
	}
}