using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.UpdateTodo;

public record UpdateTodoCommand(
	Guid TodoId,
	string Title,
	string Description,
	DateTime DueDate,
	int Priority,
	string Status
	) : ICommand<ErrorOr<Unit>>;