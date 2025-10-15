using ToDoManager.Domain.Common.Models;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Domain.Todos.Entities;

public sealed class SubTodo : Entity<SubTodoId>
{
	public string Title { get; private set; }
	public string Description { get; private set; }
	public bool IsComplete { get; private set; }
	public int Order { get; private set; }
	
	private SubTodo(
		SubTodoId id, 
		string title, 
		string description, 
		bool isComplete, 
		int order
		) : base(id)
	{
		Title = title;
		Description = description;
		IsComplete = isComplete;
		Order = order;
	}

	public static SubTodo Create(string title, string description, bool isComplete, int order)
	{
		return new SubTodo(SubTodoId.CreateUnique(), title, description, isComplete, order);
	}
	
#pragma warning disable CS8618
	public SubTodo() { }
#pragma warning restore CS8618
}