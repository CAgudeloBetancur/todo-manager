using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Application.Tags.Common;
using ToDoManager.Domain.Tags;

namespace ToDoManager.Application.Tags.Commands.CreateTag;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, ErrorOr<DefaultTagResult>>
{
	private readonly ITagRepository _tagRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
	{
		_tagRepository = tagRepository;
		_unitOfWork = unitOfWork;
	}
	
	public async Task<ErrorOr<DefaultTagResult>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
	{
		var tag = Tag.Create(request.Name);

		await _tagRepository.AddAsync(tag);

		var persistenceResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
		
		var persistenceResultCheck = EnsurePersistenceSucceeded(persistenceResult);

		return BuildFinalResult(persistenceResultCheck, tag);
	}

	private ErrorOr<Unit> EnsurePersistenceSucceeded(Error? persistenceResult)
	{
		return persistenceResult is not null 
			? (Error)persistenceResult 
			: Unit.Value;
	}

	private ErrorOr<DefaultTagResult> BuildFinalResult(
		ErrorOr<Unit> persistenceResultCheck,
		Tag tag
		)
	{
		return persistenceResultCheck.IsError
			? persistenceResultCheck.Errors
			: new DefaultTagResult(tag.Id.Value, tag.Name);
	}
}