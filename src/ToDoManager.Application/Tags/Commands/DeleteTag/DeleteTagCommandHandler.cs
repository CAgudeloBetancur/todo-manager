using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.DeleteTag;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, ErrorOr<Unit>>
{
	private readonly ITagRepository _tagRepository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
	{
		_tagRepository = tagRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<ErrorOr<Unit>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
	{
		var tagId = ToTagId(request.TagId);
		
		var tagResult = await GetTagByIdAsync(tagId);

		if (tagResult.IsError) return tagResult.Errors;
		
		var tag = tagResult.Value;
		
		await _tagRepository.RemoveAsync(tag);
		
		return Unit.Value;
	}

	private static TagId ToTagId(Guid primitiveTagId) => TagId.Create(primitiveTagId);

	private async Task<ErrorOr<Tag>> GetTagByIdAsync(TagId tagId)
	{
		var tag = await _tagRepository.GetByIdAsync(tagId);
		return tag is null ? Errors.Tag.NotFound : tag;
	}
}