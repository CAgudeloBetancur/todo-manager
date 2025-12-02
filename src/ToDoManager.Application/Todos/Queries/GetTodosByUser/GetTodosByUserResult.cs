namespace ToDoManager.Application.Todos.Queries.GetTodosByUser;

public record class GetTodosByUserResult(
	Guid Id,
	string Title,
	string Description,
	string Status,
	string PriorityName,
	int PriorityValue,
	DateTime DueDate,
	Guid UserId,
	DateTime CreatedAt,
	List<SubTodoResponse> SubTodos,
	List<Guid> TagIds
	);
	
public record class SubTodoResponse(
	Guid Id,
	string Title,
	string Description
	);