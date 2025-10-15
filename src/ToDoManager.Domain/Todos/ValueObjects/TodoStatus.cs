using ToDoManager.Domain.Common.Models;

namespace ToDoManager.Domain.Todos.ValueObjects;

public sealed class TodoStatus : ValueObject
{
	public static readonly TodoStatus Pending = new("Pending");
	public static readonly TodoStatus InProgress = new("InProgress");
	public static readonly TodoStatus Done = new("Done");
	public static readonly TodoStatus Cancelled = new("Cancelled");
	
	public static IEnumerable<TodoStatus> All => new[]
	{
		Pending, InProgress, Done, Cancelled
	};
	
	public string Value { get; private set; }

	private TodoStatus(string value)
	{
		Value = value;
	}
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}

	public static TodoStatus From(string value)
	{
		var status = All.FirstOrDefault(x => x.Value == value);

		return status ?? throw new ArgumentException($"Invalid status {value}" );
	}

	public override string ToString() => Value;
	
#pragma warning disable CS8618
	public TodoStatus() { }
#pragma warning restore CS8618
}