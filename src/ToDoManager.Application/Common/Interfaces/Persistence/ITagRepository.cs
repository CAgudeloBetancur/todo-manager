using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Common.Interfaces.Persistence;

public interface ITagRepository
{
	Task<Tag?> GetByIdAsync(TagId id);
	Task<List<Tag>> GetByIdsAsync(IEnumerable<TagId> ids);
	Task AddAsync(Tag tag);
	Task RemoveAsync(Tag tag);
	Task<List<Tag>> GetAllAsync();
	Task Update(Tag tag);
}