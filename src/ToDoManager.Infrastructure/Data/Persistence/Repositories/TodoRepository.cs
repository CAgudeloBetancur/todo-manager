using Microsoft.EntityFrameworkCore;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Infrastructure.Data.Persistence.Repositories;

public class TodoRepository : ITodoRepository
{
	private readonly ApplicationDbContext _context;

	public TodoRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task AddAsync(Todo todo)
	{
		_context.Todos.Add(todo);
	}

	public async Task<Todo?> GetByIdForUserAsync(TodoId id, UserId ownerId)
	{
		return await _context
			.Todos
			.Include(x => x.SubTodos)
			.SingleOrDefaultAsync(
				t => t.Id == id && EF.Property<Guid>(t, "_ownerIdGuid") == ownerId.Value
				);
	}

	public async Task<List<Todo>> ListByUserAsync(UserId userId)
	{
		return await _context
			.Todos
			.Include(t => t.SubTodos)
			.Include(t => t.TagIds)
			.Where(t => EF.Property<Guid>(t, "_ownerIdGuid") == userId.Value)
			.ToListAsync();
	}

	public async Task Remove(Todo todo)
	{
		_context.Todos.Remove(todo);
	}

	public async Task Update(Todo todo)
	{
		_context.Todos.Update(todo);
	}
}