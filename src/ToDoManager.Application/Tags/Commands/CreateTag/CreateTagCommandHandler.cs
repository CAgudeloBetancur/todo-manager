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

	public CreateTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
	{
		_tagRepository = tagRepository;
	}
	
	public async Task<ErrorOr<DefaultTagResult>> Handle(
		CreateTagCommand request, 
		CancellationToken cancellationToken
		)
	{
		var tag = Tag.Create(request.Name);

		await _tagRepository.AddAsync(tag);

		return BuildFinalResult(tag);
	}

	private static ErrorOr<DefaultTagResult> BuildFinalResult(Tag tag)
	{
			return new DefaultTagResult(tag.Id.Value, tag.Name);
	}
}