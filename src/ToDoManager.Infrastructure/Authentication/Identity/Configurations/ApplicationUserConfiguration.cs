using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoManager.Domain.Users.ValueObjects;
using ToDoManager.Infrastructure.Authentication.Identity.Entities;

namespace ToDoManager.Infrastructure.Authentication.Identity.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
	public void Configure(EntityTypeBuilder<ApplicationUser> builder)
	{
		builder
			.HasKey(u => u.Id);
		
		builder
			.Property(u => u.Id)
			.ValueGeneratedNever();
		
		builder
			.Property(u => u.CreatedAt)
			.HasDefaultValueSql("NOW()");
		
		builder
			.Property(u => u.CreatedAt)
			.HasConversion(
				createdAt => createdAt.ToUniversalTime(), 
				value => DateTime.SpecifyKind(value, DateTimeKind.Utc)
				);
	}
}