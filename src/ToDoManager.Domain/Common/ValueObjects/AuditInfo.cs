using ToDoManager.Domain.Common.Models;

namespace ToDoManager.Domain.Common.ValueObjects;

public sealed class AuditInfo : ValueObject
{
	public Guid CreatedBy { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public Guid? ModifiedBy { get; private set; }
	public DateTime? ModifiedAt { get; private set; }

	private AuditInfo(Guid createdBy, DateTime createdAt)
	{
		CreatedBy = createdBy;
		CreatedAt = createdAt;
	}

	public static AuditInfo Create(Guid createdBy, DateTime createdAt)
	{
		return new(createdBy, createdAt);
	}

	public void Modify(Guid modifiedBy, DateTime modifiedAt)
	{
		ModifiedBy = modifiedBy;
		ModifiedAt = modifiedAt;
	}
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return CreatedBy;
		yield return CreatedAt;
	}

#pragma warning disable CS8618
	public AuditInfo() {	}
#pragma warning restore CS8618
}