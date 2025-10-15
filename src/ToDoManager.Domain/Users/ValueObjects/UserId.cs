using ToDoManager.Domain.Common.Models;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Domain.Users.ValueObjects;

public sealed class UserId : ValueObject
{
	public Guid Value { get; private set; }

	private UserId(Guid value)
	{
		Value = value;
	}

	public static UserId CreateUnique() => new(Guid.NewGuid());
	public static UserId Create(Guid value) => new(value);
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}
	
#pragma warning disable CS8618
	public UserId() { }
#pragma warning restore CS8618
}