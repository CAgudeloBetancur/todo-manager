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

	public static Tag Create(string name) => new(TagId.CreateUnique(), name);
	
	public static Tag Create(TagId tagId, string name) => new(tagId, name);

	public void Update(string name)
	{
		UpdateName(name);
	}

	private void UpdateName(string newName)
	{
		Name = newName;
	} 
	
#pragma warning restore CS8618
	public Tag() { }
#pragma warning disable CS8618
}