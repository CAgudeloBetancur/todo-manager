namespace ToDoManager.Contracts.Todos;

public record class CreateTodoRequest(
	string Title,
	string Description,
	string Status,
	int? Priority,
	string? DueDate
	);