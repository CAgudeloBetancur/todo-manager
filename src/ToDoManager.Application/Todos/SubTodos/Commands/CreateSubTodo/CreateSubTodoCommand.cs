using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.SubTodos.Commands.CreateSubTodo;

public record CreateSubTodoCommand(
	Guid TodoId,
	string Title, 
	string Description, 
	bool IsComplete
	) : IRequest<ErrorOr<Unit>>;