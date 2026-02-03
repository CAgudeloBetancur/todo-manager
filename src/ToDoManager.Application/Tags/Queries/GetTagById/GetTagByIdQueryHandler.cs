using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Tags.Common;
using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Queries.GetTagByIdQuery;

public class GetTagByIdQueryHandler : IRequestHandler<GetTagById.GetTagByIdQuery, ErrorOr<DefaultTagResult>>
{
	private readonly ITagRepository _tagRepository;

	public GetTagByIdQueryHandler(ITagRepository tagRepository)
	{
		_tagRepository = tagRepository;
	}

	public async Task<ErrorOr<DefaultTagResult>> Handle(GetTagById.GetTagByIdQuery request, CancellationToken cancellationToken)
	{
		var tagId = ToTagId(request.TagId);
		
		var tagResult = await GetTagByIdAsync(tagId);

		if (tagResult.IsError) return tagResult.Errors;

		var tag = tagResult.Value;

		return MapToResult(tag);
	}

	private static TagId ToTagId(Guid primitiveTagId) => TagId.Create(primitiveTagId);

	private async Task<ErrorOr<Tag>> GetTagByIdAsync(TagId tagId)
	{
		var tag = await _tagRepository.GetByIdAsync(tagId);
		
		return tag is null ? Errors.Tag.NotFound : tag;
	}

	private DefaultTagResult MapToResult(Tag tag)
	{
		return new DefaultTagResult(tag.Id.Value, tag.Name);
	}
}