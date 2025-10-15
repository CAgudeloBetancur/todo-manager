using ToDoManager.Domain.Common.Models;

namespace ToDoManager.Domain.Todos.ValueObjects;

public sealed class SubTodoId : ValueObject
{
	public Guid Value { get; private set; }

	private SubTodoId(Guid value)
	{
		Value = value;
	}

	public static SubTodoId CreateUnique() => new(Guid.NewGuid());
	public static SubTodoId Create(Guid value) => new(value);
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}
	
#pragma warning disable CS8618
	public SubTodoId() { }
#pragma warning restore CS8618
}