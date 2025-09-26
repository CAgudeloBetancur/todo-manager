using Microsoft.AspNetCore.Identity;

namespace ToDoManager.Infrastructure.Data.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}