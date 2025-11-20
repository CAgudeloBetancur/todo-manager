using ToDoManager.Domain.Common.Models;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
	public string DisplayName { get; private set; }
	public Email Email { get; private set; }
	
	public string FirstName { get; private set; }
	public string LastName { get; private set; }
	
	private User(UserId id, string displayName, Email email, string firstName, string lastName) : base(id)
	{
		DisplayName = displayName;
		Email = email;
		FirstName = firstName;
		LastName = lastName;
	}

	public static User Create(string displayName, string email, string firstName, string lastName)
	{
		return new (UserId.CreateUnique(), displayName, Email.Create(email), firstName, lastName);
	}
	
	public static User Create(Guid id, string displayName, string email, string firstName, string lastName)
	{
		return new (UserId.Create(id), displayName, Email.Create(email), firstName, lastName);
	}
	
#pragma warning disable CS8618
	public User(string firstName, string lastName)
	{
		FirstName = firstName;
		LastName = lastName;
	}
#pragma warning restore CS8618
}