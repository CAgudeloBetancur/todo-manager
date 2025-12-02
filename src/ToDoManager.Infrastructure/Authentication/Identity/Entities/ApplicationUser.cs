using Microsoft.AspNetCore.Identity;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Infrastructure.Authentication.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public string FirstName { get; set; }
	public string LastName { get; set; }

	public ICollection<Todo> Todos { get; set; } = new List<Todo>();
}