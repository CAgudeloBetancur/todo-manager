using ToDoManager.Domain.Common.Models;

namespace ToDoManager.Domain.Todos.ValueObjects;

public class TodoPriority : ValueObject
{
	public int Value { get; private set; }

	public string Label => Value switch
	{
		<= 33 => "Low",
		<= 66 => "Medium",
		_ => "High"
	};

	public bool IsUrgent => Value >= 80;

	private TodoPriority(int value)
	{
		if(value is < 1 or > 100) 
			throw new ArgumentOutOfRangeException(
				nameof(value), 
				"Value must be between 1 and 100."
				);
		
		Value = value;
		
	}
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
		yield return Label;
	}

	public static TodoPriority Create(int value)
	{
		return new(value);
	}

	public bool IsHigherThan(TodoPriority other) => Value > other.Value;

	public override string ToString() => Label;
	
#pragma warning disable CS8618
	public TodoPriority() {	}
#pragma warning restore CS8618
}