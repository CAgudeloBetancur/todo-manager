using ToDoManager.Domain.Common.Models;

namespace ToDoManager.Domain.Users.ValueObjects;

public sealed class Email : ValueObject
{
	public string Value { get; private set; }

	private Email(string value)
	{
		Value = value;
	}

	public static Email Create(string email)
	{
		// validar formato correo
		
		return new(email);
	} 
	
	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Value;
	}
	
#pragma warning disable CS8618
	public Email() {}
#pragma warning restore CS8618
}