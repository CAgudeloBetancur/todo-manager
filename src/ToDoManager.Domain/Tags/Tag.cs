using ToDoManager.Domain.Common.Models;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Domain.Tags;

public sealed class Tag : AggregateRoot<TagId>
{
	public string Name { get; private set; }
	
	private Tag(TagId id, string name) : base(id)
	{
		Name = name;
	}

	public static Tag Create(string name, TodoId todoId){
		return new(TagId.CreateUnique(), name);
	}
	
#pragma warning restore CS8618
	public Tag() { }
#pragma warning disable CS8618
}