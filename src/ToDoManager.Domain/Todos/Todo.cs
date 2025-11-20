using ToDoManager.Domain.Common.Models;
using ToDoManager.Domain.Common.ValueObjects;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos.Entities;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Domain.Todos;

public sealed class Todo : AggregateRoot<TodoId>
{
	public string Title { get; private set; }
	public string Description { get; private set; }
	public TodoStatus Status { get; private set; }
	public TodoPriority Priority { get; private set; }
	public DueDate DueDate { get; private set; }
	public UserId OwnerId { get; private set; }
	public AuditInfo AuditInfo { get; private set; } 
	
	private readonly List<SubTodo> _subTodos = new();
	private readonly List<TagId> _tagIds = new();
	
	public IReadOnlyList<SubTodo> SubTodos => _subTodos.AsReadOnly();
	public IReadOnlyList<TagId> TagIds => _tagIds.AsReadOnly();

	private Todo(
		TodoId id,
		string title,
		string description,
		TodoStatus status,
		TodoPriority priority,
		DueDate dueDate,
		UserId ownerId,
		AuditInfo auditInfo
		) : base(id)
	{
		Title = title;
		Description = description;
		Status = status;
		Priority = priority;
		DueDate = dueDate;
		OwnerId = ownerId;
		AuditInfo = auditInfo;
	}

	public static Todo Create(
		string title,
		string description,
		TodoStatus status,
		TodoPriority priority,
		DueDate dueDate,
		UserId ownerId,
		AuditInfo auditInfo
		)
	{
		return new(TodoId.CreateUnique(), title, description, status, priority, dueDate, ownerId, auditInfo);
	}
	
	public static Todo Create(
		TodoId id,
		string title,
		string description,
		TodoStatus status,
		TodoPriority priority,
		DueDate dueDate,
		UserId ownerId,
		AuditInfo auditInfo
	)
	{
		return new(id, title, description, status, priority, dueDate, ownerId, auditInfo);
	}

#pragma warning disable CS8618
	public Todo() {	}
#pragma warning restore CS8618
}