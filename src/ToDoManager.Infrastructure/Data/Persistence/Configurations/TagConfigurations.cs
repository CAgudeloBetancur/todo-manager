using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Infrastructure.Data.Persistence.Configurations;

public class TagConfigurations : IEntityTypeConfiguration<Tag>
{
	public void Configure(EntityTypeBuilder<Tag> builder)
	{
		builder
			.ToTable("Tags");
		
		builder
			.HasKey(t => t.Id);

		builder
			.Property(t => t.Id)
			.ValueGeneratedNever()
			.HasConversion(id => id.Value, value => TagId.Create(value));

		builder
			.Property(t => t.Name)
			.HasMaxLength(100)
			.IsRequired();
	}
}