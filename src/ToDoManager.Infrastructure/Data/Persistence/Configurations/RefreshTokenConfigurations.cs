using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoManager.Infrastructure.Authentication.Jwt.Models;

namespace ToDoManager.Infrastructure.Data.Persistence.Configurations;

public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder
			.ToTable("RefreshTokens");
		
		builder
			.HasKey(rt => rt.Id);
		
		builder
			.Property(rt => rt.Token)
			.IsRequired()
			.HasMaxLength(200);
		
		builder
			.Property(rt => rt.ExpiresAt)
			.IsRequired();
		
		builder
			.Property(rt => rt.UserId)
			.IsRequired();
		
		builder
			.Property(rt => rt.IsRevoked)
			.IsRequired();
			
	}
}