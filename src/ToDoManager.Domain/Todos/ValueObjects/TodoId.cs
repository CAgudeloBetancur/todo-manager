using ToDoManager.Domain.Common.Models;

namespace ToDoManager.Domain.Todos.ValueObjects;

public sealed class TodoId : ValueObject
{
	public Guid Value { get; private set; }

	private TodoId(Guid value)
	{
		Value = value;
	}

	public static TodoId CreateUnique() => new(Guid.NewGuid());
	public static TodoId Create(Guid value) => new(value);
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}
	
#pragma warning disable CS8618
	public TodoId() {	}
#pragma warning restore CS8618
}