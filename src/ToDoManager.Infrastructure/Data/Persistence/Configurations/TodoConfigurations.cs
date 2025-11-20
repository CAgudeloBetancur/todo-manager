using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.Entities;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Infrastructure.Data.Persistence.Configurations;

public class TodoConfigurations : IEntityTypeConfiguration<Todo>
{
	public void Configure(EntityTypeBuilder<Todo> builder)
	{
		ConfigureTodoTable(builder);
		ConfigureSubTodoTable(builder);
		ConfigureTagIdsTable(builder);
	}

	private void ConfigureTagIdsTable(EntityTypeBuilder<Todo> builder)
	{
		builder
			.OwnsMany(t => t.TagIds, tagIdsBuilder =>
			{
				tagIdsBuilder
					.ToTable("TodoTagIds");

				tagIdsBuilder
					.WithOwner()
					.HasForeignKey("TodoId");

				tagIdsBuilder
					.HasKey("Id");
				
				tagIdsBuilder
					.Property(tg => tg.Value)
					.HasColumnName("TagId")
					.ValueGeneratedNever();
			});
		
		builder
			.Metadata
			.FindNavigation(nameof(Todo.TagIds))!
			.SetPropertyAccessMode(PropertyAccessMode.Field);
	}

	private void ConfigureSubTodoTable(EntityTypeBuilder<Todo> builder)
	{
		builder
			.OwnsMany(t => t.SubTodos, subTodoBuilder =>
				{
					subTodoBuilder
						.ToTable("SubTodos");

					subTodoBuilder
						.WithOwner()
						.HasForeignKey("TodoId");
					
					subTodoBuilder
						.Property(t => t.Id)
						.HasColumnName("SubTodoId")
						.ValueGeneratedNever()
						.HasConversion(id => id.Value, value => SubTodoId.Create(value));
					
					subTodoBuilder
						.HasKey(nameof(SubTodo.Id), "TodoId");

					subTodoBuilder
						.Property(t => t.Title)
						.HasMaxLength(100);
					
					subTodoBuilder
						.Property(t => t.Description)
						.HasMaxLength(100);

					subTodoBuilder
						.Property(t => t.IsComplete)
						.HasDefaultValue(false)
						.IsRequired();
					
					subTodoBuilder
						.Property(t => t.Order)
						.IsRequired();
				});
		
		builder
			.Metadata
			.FindNavigation(nameof(Todo.SubTodos))!
			.SetPropertyAccessMode(PropertyAccessMode.Field);
	}

	private void ConfigureTodoTable(EntityTypeBuilder<Todo> builder)
	{
		builder
			.ToTable("Todos");

		builder
			.HasKey(t => t.Id);

		builder
			.Property(t => t.Id)
			.ValueGeneratedNever()
			.HasConversion( id => id.Value, value => TodoId.Create(value) );
		
		builder
			.Property(t => t.Title)
			.HasMaxLength(100)
			.IsRequired();
		
		builder
			.Property(t => t.Description)
			.HasMaxLength(100)
			.IsRequired();
		
		builder
			.Property(t => t.Status)
			.HasConversion(status => status.Value, value => TodoStatus.From(value));
		
		builder
			.Property(t => t.Priority)
			.HasConversion(priority => priority.Value, value => TodoPriority.Create(value));
		
		builder
			.Property(t => t.DueDate)
			.HasConversion(
				dueDate => DateTime.SpecifyKind(dueDate.Value, DateTimeKind.Utc), 
				value => DueDate.Create(DateTime.SpecifyKind(value, DateTimeKind.Utc)));
		
		builder
			.Property(t => t.OwnerId)
			.HasConversion(userId => userId.Value, value => UserId.Create(value));

		builder
			.OwnsOne(t => t.AuditInfo, auditInfo =>
			{
				auditInfo
					.WithOwner();
			});
	}
}