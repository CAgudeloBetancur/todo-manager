using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.AddTagIdToTodo;

public class AddTagIdCommandHandler : IRequestHandler<AddTagIdToTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly ITagRepository _tagRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserAccessor _userAccessor;

	public AddTagIdCommandHandler(
		ITodoRepository todoRepository, 
		ITagRepository tagRepository, 
		IUnitOfWork unitOfWork, 
		IUserAccessor userAccessor
		)
	{
		_todoRepository = todoRepository;
		_tagRepository = tagRepository;
		_unitOfWork = unitOfWork;
		_userAccessor = userAccessor;
	}
	
	public async Task<ErrorOr<Unit>> Handle(AddTagIdToTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();
		
		if(currentUserId is null) return Errors.User.NotFound;

		var todo = await _todoRepository
			.GetByIdForUserAsync(
				TodoId.Create(request.TodoId), 
				UserId.Create((Guid)currentUserId)
				);

		if (todo is null) return Errors.ToDo.NotFound;
		
		var tag = await _tagRepository.GetByIdAsync(TagId.Create(request.TagId));

		if (tag is null) return Errors.Tag.NotFound;
		
		todo.AddTagId(tag.Id);
		
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}