using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.Commands.CreateTodo;

public record class CreateTodoCommand(
	string Title,
	string? Description,
	DateTime? DueDate,
	int? Priority,
	string? Status
	) : IRequest<ErrorOr<CreateTodoResult>>;