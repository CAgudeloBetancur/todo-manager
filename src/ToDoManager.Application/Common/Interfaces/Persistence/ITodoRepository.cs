using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Common.Interfaces.Persistence;

public interface ITodoRepository
{
	Task AddAsync(Todo todo);
	Task<Todo?> GetByIdForUserAsync(TodoId todoId,UserId ownerId);
	Task<List<Todo>> ListByUserAsync(UserId userId);
	Task Remove(Todo todo);
	Task Update(Todo todo);
}