using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, ErrorOr<Unit>>
{
	private readonly ITagRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateTagCommandHandler(IUnitOfWork unitOfWork, ITagRepository repository)
	{
		_unitOfWork = unitOfWork;
		_repository = repository;
	}

	public async Task<ErrorOr<Unit>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
	{
		var tagId = ToTagId(request.TagId);
		
		var tagResult = await GetTagByidAsync(tagId);

		if (tagResult.IsError) return tagResult.Errors;
		
		var currentTag = tagResult.Value;

		currentTag.Update(request.Name);
		
		await _repository.Update(currentTag);

		return Unit.Value;
	}

	private static TagId ToTagId(Guid primitiveTagId) => TagId.Create(primitiveTagId);

	private async Task<ErrorOr<Tag>> GetTagByidAsync(TagId tagId)
	{
		var tag = await _repository.GetByIdAsync(tagId);
		return tag is null ? Errors.Tag.NotFound : tag;
	} 
}