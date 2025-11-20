namespace ToDoManager.Application.Todos.Commands.CreateTodo;

public record class CreateTodoResult(
	Guid Id,
	string Title,
	string Description,
	string Status,
	string Priority,
	DateTime DueDate,
	Guid OwnerId,
	DateTime CreatedAt
	);