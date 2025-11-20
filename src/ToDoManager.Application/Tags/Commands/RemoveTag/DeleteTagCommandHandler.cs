using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.RemoveTag;

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
		var currentTag = await _tagRepository.GetByIdAsync(TagId.Create(request.Id));

		if (currentTag is null) return Errors.Tag.NotFound;
		
		await _tagRepository.RemoveAsync(currentTag);

		var error = await _unitOfWork.SaveChangesAsync(cancellationToken);
		
		if(error is not null) return (Error)error;

		return Unit.Value;
	}
}