using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.SubTodos.Commands.UpdateSubTodo;

public record UpdateSubTodoCommand(
	Guid TodoId, 
	Guid SubTodoId, 
	string Title, 
	string Description, 
	bool IsComplete
	) : IRequest<ErrorOr<Unit>>;