using Microsoft.EntityFrameworkCore;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Infrastructure.Data.Persistence.Repositories;

public class TagRepository : ITagRepository
{
	private readonly ApplicationDbContext _context;

	public TagRepository(ApplicationDbContext context)
	{
		_context = context;
	}
	
	public async Task<Tag?> GetByIdAsync(TagId id)
	{
		return await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
	}

	public async Task<List<Tag>> GetByIdsAsync(IEnumerable<TagId> ids)
	{
		return await _context
			.Tags
			.Where(t => ids.Contains(t.Id))
			.ToListAsync();
	}

	public async Task AddAsync(Tag tag)
	{
		await _context.Tags.AddAsync(tag);
	}

	public async Task RemoveAsync(Tag tag)
	{
		_context.Tags.Remove(tag);
	}

	public async Task<List<Tag>> GetAllAsync()
	{
		return await _context.Tags.ToListAsync();
	}

	public async Task Update(Tag currentTag, Tag updatedTag)
	{
		_context.Entry(currentTag).CurrentValues.SetValues(updatedTag);
	}
}