using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.Commands.UpdateTodo;

public record UpdateTodoCommand(
	Guid TodoId,
	string Title,
	string Description,
	DateTime DueDate,
	int Priority,
	string Status
	) : IRequest<ErrorOr<Unit>>;