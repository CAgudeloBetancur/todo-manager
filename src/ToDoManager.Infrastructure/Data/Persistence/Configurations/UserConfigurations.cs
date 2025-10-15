using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;
using ToDoManager.Infrastructure.Data.Identity.Entities;

namespace ToDoManager.Infrastructure.Data.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		ConfigureUserTable(builder);
	}

	private void ConfigureUserTable(EntityTypeBuilder<User> builder)
	{
		builder
			.ToTable("Users");

		builder
			.HasKey(u => u.Id);

		builder
			.Property(u => u.Id)
			.ValueGeneratedNever()
			.HasConversion(id => id.Value, value => UserId.Create(value));

		builder
			.Property(u => u.DisplayName)
			.HasMaxLength(100);

		builder
			.Property(u => u.Email)
			.HasConversion(email => email.Value, value => Email.Create(value));

		builder
			.HasOne<ApplicationUser>()
			.WithMany()
			.HasForeignKey(u => u.IdentityUserId)
			.OnDelete(DeleteBehavior.Restrict);

	}
}