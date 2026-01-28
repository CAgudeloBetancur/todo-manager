using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Tags.Common;
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
		var tag = await _tagRepository.GetByIdAsync(TagId.Create(request.Id));
		
		if(tag is null) return Errors.Tag.NotFound;

		return new DefaultTagResult(tag.Id.Value, tag.Name);
	}
}