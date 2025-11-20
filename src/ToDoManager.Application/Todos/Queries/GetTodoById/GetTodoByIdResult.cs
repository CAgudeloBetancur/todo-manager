namespace ToDoManager.Application.Todos.Queries.GetTodoById;

public record class GetTodoByIdResult(
	Guid Id,
	string Title,
	string Description,
	string Status,
	int PriorityValue,
	string PriorityName,
	DateTime DueDate,
	Guid OwnerId,
	DateTime CreatedAt
	);