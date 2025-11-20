using Microsoft.AspNetCore.Identity;

namespace ToDoManager.Infrastructure.Authentication.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public string FirstName { get; set; }
	public string LastName { get; set; }
}