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
		var tagResult = await GetTagByIdAsync(request.TagId);

		if (tagResult.IsError) return tagResult.Errors;
		
		var tag = tagResult.Value;
		
		await _tagRepository.RemoveAsync(tag);

		var persistenceResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
		
		return BuildFinalResultFromPersistenceResult(persistenceResult);
	}

	private async Task<ErrorOr<Tag>> GetTagByIdAsync(TagId tagId)
	{
		var tag = await _tagRepository.GetByIdAsync(tagId);
		return tag is null ? Errors.Tag.NotFound : tag;
	}

	private static ErrorOr<Unit> BuildFinalResultFromPersistenceResult(Error? persistenceResult)
	{
		return persistenceResult is not null 
			? (Error)persistenceResult 
			: Unit.Value;
	}
}