using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.RemoveTagIdFromTodo;

public class RemoveTagIdFromTodoCommandHandler : IRequestHandler<RemoveTagIdFromTodoCommand, ErrorOr<Unit>>
{
	private readonly IUserAccessor _userAccessor;
	private readonly ITodoRepository _todoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public RemoveTagIdFromTodoCommandHandler(IUserAccessor userAccessor, ITodoRepository todoRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork)
	{
		_userAccessor = userAccessor;
		_todoRepository = todoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<ErrorOr<Unit>> Handle(RemoveTagIdFromTodoCommand request, CancellationToken cancellationToken)
	{
		var userId = _userAccessor.GetId();
		
		if(userId is null) return Errors.User.NotFound;

		var todo = await _todoRepository.GetByIdForUserAsync(TodoId.Create(request.TodoId), UserId.Create((Guid)userId));

		if (todo is null) return Errors.ToDo.NotFound;
		
		todo.RemoveTagId(TagId.Create(request.TagId));

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}