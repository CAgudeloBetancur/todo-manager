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
	private readonly Guid _ownerIdGuid;
	// public UserId OwnerId { get; private set; }
	public UserId OwnerId => UserId.Create(_ownerIdGuid);
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
		AuditInfo = auditInfo;
		_ownerIdGuid = ownerId.Value;
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

	public void UpdateTitle(string newTitle)
	{
		Title = newTitle;
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}

	public void UpdateDescription(string newDescription)
	{
		Description = newDescription;
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}

	public void UpdateStatus(TodoStatus newStatus)
	{
		Status = newStatus;
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}

	public void UpdatePriority(TodoPriority newPriority)
	{
		Priority = newPriority;
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}

	public void UpdateDueDate(DueDate newDueDate)
	{
		if (newDueDate.Value < DateTimeOffset.UtcNow)
		{
			throw new Exception("Due date cannot be in the past");
		}
		
		DueDate = newDueDate;
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}
	
	public void AddSubTodo(string title, string description, bool isComplete)
	{
		var nextOrder = SubTodos.Any() ? SubTodos.Max(x => x.Order) + 1 : 1;
		
		var subTodo = SubTodo.Create(title, description, isComplete, nextOrder);
		
		_subTodos.Add(subTodo);
		
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}

	public void UpdateSubTodo(SubTodoId subTodoId, string title, string description, bool isComplete)
	{
		var subTodo = _subTodos.FirstOrDefault(s => s.Id == subTodoId);

		if (subTodo is null) throw new Exception("SubTodo not found");
		
		var subTodoChanged = 
			subTodo.Title != title ||
			subTodo.Description != description ||
			subTodo.IsComplete != isComplete;
		
		if(subTodoChanged) subTodo.Update(title, description, isComplete);
		
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}

	public void RemoveSubTodo(SubTodoId subTodoId)
	{
		var subTodo = _subTodos.FirstOrDefault(s => s.Id == subTodoId);
		
		if (subTodo is null) throw new Exception("SubTodo not found");
		
		_subTodos.Remove(subTodo);

		var reorderedSubTodos = SubTodos.OrderBy(st => st.Order).ToList();
		
		for (var i = 0; i < reorderedSubTodos.Count; i++) reorderedSubTodos[i].UpdateOrder(i + 1);
		
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}

	public void ReorderSubTodos(List<(Guid Id, int Order)> newOrderList)
	{
		foreach (var item in newOrderList)
		{
			var subTodo = _subTodos.FirstOrDefault(x => x.Id.Value == item.Id)
				?? throw new Exception("SubTodo not found");

			if(subTodo.Order != item.Order) subTodo.UpdateOrder(item.Order);
		}
		
		var orders = _subTodos.Select(x => x.Order).OrderBy(x => x).ToList();

		if (!orders.SequenceEqual(Enumerable.Range(1, _subTodos.Count)))
		{
			throw new Exception("Invalid order sequence");
		}
		
		AuditInfo.UpdateModifiedAt(DateTime.UtcNow);
	}
	
	public void AddTagId(TagId tagId)
	{
		if(TagIds.Contains(tagId)) throw new Exception("Tag already exists.");
		
		_tagIds.Add(tagId);
	}
	
	public void RemoveTagId(TagId tagId)
	{
		if(!TagIds.Contains(tagId)) throw new Exception("Tag not found.");
		
		_tagIds.Remove(tagId);
	}

	public void ClearTagIds()
	{
		_tagIds.Clear();
	}

#pragma warning disable CS8618
	public Todo() {	}
#pragma warning restore CS8618
}