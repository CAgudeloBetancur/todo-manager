using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Todos.Commands.CreateTodo;

public record class CreateTodoCommand(
	string Title,
	string? Description,
	DateTime? DueDate,
	int? Priority,
	string? Status
	) : ICommand<ErrorOr<CreateTodoResult>>;