using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoManager.Infrastructure.Data.Identity.Entities;

namespace ToDoManager.Infrastructure.Data.Identity.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
	public void Configure(EntityTypeBuilder<ApplicationUser> builder)
	{
		builder.Property(u => u.CreatedAt)
			.HasDefaultValueSql("NOW()");
	}
}