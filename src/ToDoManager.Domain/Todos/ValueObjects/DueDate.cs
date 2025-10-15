using ToDoManager.Domain.Common.Models;

namespace ToDoManager.Domain.Todos.ValueObjects;

public sealed class DueDate : ValueObject
{
	public DateTime Value { get; private set; }

	private DueDate(DateTime dueDate)
	{
		Value = dueDate;
	}

	public static DueDate Create(DateTime dueDate) => new(dueDate);
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}
	
#pragma warning disable CS8618
	public DueDate() {	}
#pragma warning restore CS8618
}