using ToDoManager.Domain.Common.Models;

namespace ToDoManager.Domain.Tags.ValueObjects;

public sealed class TagId : ValueObject
{
	public Guid Value { get; private set; }

	private TagId(Guid value)
	{
		Value = value;
	}

	public static TagId CreateUnique() => new(Guid.NewGuid());
	public static TagId Create(Guid value) => new(value);
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}
	
#pragma warning disable CS8618
	public TagId() {	}
#pragma warning restore CS8618
}