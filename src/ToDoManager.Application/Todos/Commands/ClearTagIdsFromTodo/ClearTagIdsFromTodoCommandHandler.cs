using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.ClearTagIdsFromTodo;

public class ClearTagIdsFromTodoCommandHandler : IRequestHandler<ClearTagIdsFromTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IUnitOfWork _unitOfWork;

	public ClearTagIdsFromTodoCommandHandler(ITodoRepository todoRepository, IUserAccessor userAccessor, IUnitOfWork unitOfWork)
	{
		_todoRepository = todoRepository;
		_userAccessor = userAccessor;
		_unitOfWork = unitOfWork;
	}
	
	public async Task<ErrorOr<Unit>> Handle(ClearTagIdsFromTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();

		if (currentUserId is null) return Errors.User.NotFound;
		
		var todo = await _todoRepository.GetByIdForUserAsync(TodoId.Create(request.TodoId), UserId.Create(currentUserId.Value));

		if (todo is null) return Errors.ToDo.NotFound;
		
		todo.ClearTagIds();

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}