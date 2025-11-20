using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Tags.Common;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Queries.ListTags;

public class ListTagsQueryHandler : IRequestHandler<ListTagsQuery, List<DefaultTagResult>>
{
	private readonly ITagRepository _tagRepository;

	public ListTagsQueryHandler(ITagRepository tagRepository)
	{
		_tagRepository = tagRepository;
	}
	
	public async Task<List<DefaultTagResult>> Handle(ListTagsQuery request, CancellationToken cancellationToken)
	{
		var tags = await _tagRepository.GetAllAsync();

		return tags.Select(t => new DefaultTagResult(t.Id.Value, t.Name)).ToList();
	}
}