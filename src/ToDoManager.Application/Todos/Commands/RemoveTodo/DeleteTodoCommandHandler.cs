using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.RemoveTodo;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteTodoCommandHandler(ITodoRepository todoRepository, IUnitOfWork unitOfWork, IUserAccessor userAccessor)
	{
		_todoRepository = todoRepository;
		_unitOfWork = unitOfWork;
		_userAccessor = userAccessor;
	}

	public async Task<ErrorOr<Unit>> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();

		var requestResult = await GetTodoByIdForUserAsync(request.TodoId, currentUserId);

		if(requestResult.IsError) return requestResult.Errors;

		var todo = requestResult.Value;

		await _todoRepository.Remove(todo);
		
		var persistenceResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
		
		return EnsurePersistenceResult(persistenceResult);
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUserAsync(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);
		return todo is null ? Errors.ToDo.NotFound : todo;
	}

	private static ErrorOr<Unit> EnsurePersistenceResult(Error? persistenceResult)
	{
		return persistenceResult is not null ? (Error)persistenceResult : Unit.Value;
	}
};