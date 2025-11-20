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
		var currentTag = await _repository.GetByIdAsync(TagId.Create(request.Id));

		if (currentTag is null) return Errors.Tag.NotFound;
		
		await _repository.Update(currentTag, Tag.Create(request.Id, request.Name));

		var error = await _unitOfWork.SaveChangesAsync(cancellationToken);

		if (error is not null) return (Error)error;

		return Unit.Value;
	}
}