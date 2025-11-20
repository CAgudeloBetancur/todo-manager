namespace ToDoManager.Contracts.Todos;

public record UpdateTodoRequest(
	string Title,
	string Description,
	string Status,
	int Priority,
	string DueDate
	);