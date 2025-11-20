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
	
	private AuditInfo(Guid createdBy, DateTime createdAt, Guid updatedBy, DateTime? modifiedAt)
	{
		CreatedBy = createdBy;
		CreatedAt = createdAt;
		ModifiedBy = updatedBy;
		ModifiedAt = modifiedAt;
	}

	public static AuditInfo Create(Guid createdBy, DateTime createdAt)
	{
		return new(createdBy, createdAt);
	}

	public void Update(Guid modifiedBy, DateTime modifiedAt)
    {
     	ModifiedAt = modifiedAt;
     	ModifiedBy = modifiedBy;
    }
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return CreatedBy;
		yield return CreatedAt;
		yield return ModifiedBy;
		yield return ModifiedAt;
	}

#pragma warning disable CS8618
	public AuditInfo() {	}
#pragma warning restore CS8618
}