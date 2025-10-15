using ToDoManager.Domain.Common.Models;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
	public string DisplayName { get; private set; }
	public Email Email { get; private set; }
	public Guid IdentityUserId { get; private set; }
	
	public User(UserId id) : base(id)
	{
		
	}
	
#pragma warning disable CS8618
	public User() { }
#pragma warning restore CS8618
}