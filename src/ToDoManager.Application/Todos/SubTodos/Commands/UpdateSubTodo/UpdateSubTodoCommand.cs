using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Todos.SubTodos.Commands.UpdateSubTodo;

public record UpdateSubTodoCommand(
	Guid TodoId, 
	Guid SubTodoId, 
	string Title, 
	string Description, 
	bool IsComplete
	) : ICommand<ErrorOr<Unit>>;