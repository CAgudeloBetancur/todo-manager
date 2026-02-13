using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;
using ToDoManager.Infrastructure.Authentication.Identity.Entities;
using ToDoManager.Infrastructure.Authentication.Jwt.Models;

namespace ToDoManager.Infrastructure.Data.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
	public DbSet<Todo> Todos { get; set; }
	public DbSet<Tag> Tags { get; set; }
	
	public DbSet<RefreshToken> RefreshTokens { get; set; }
	
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)	{ }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		
		builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
	}
	
}