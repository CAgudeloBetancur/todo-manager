using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Todos.SubTodos.Commands.CreateSubTodo;

public record CreateSubTodoCommand(
	Guid TodoId,
	string Title, 
	string Description, 
	bool IsComplete
	) : ICommand<ErrorOr<Unit>>;