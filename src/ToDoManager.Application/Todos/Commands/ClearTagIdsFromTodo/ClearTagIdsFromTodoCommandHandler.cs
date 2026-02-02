using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Application.Todos.Commands.CreateTodo;
using ToDoManager.Domain.Todos;
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

		var todoId = ToTodoId(request.TodoId);
		
		var queryResult = await GetTodoByIdForUserAsync(todoId, currentUserId);
		if (queryResult.IsError) return queryResult.Errors;

		var todo = queryResult.Value;
		
		todo.ClearTagIds();
		
		var persistenceResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

		return EnsurePersistenceSucceeded(persistenceResult);
	}

	private static TodoId ToTodoId(Guid primitiveTodoId)
	{
		return TodoId.Create(primitiveTodoId);
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUserAsync(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);

		return todo is null ? Errors.ToDo.NotFound : todo;
	}

	private static ErrorOr<Unit> EnsurePersistenceSucceeded(Error? persistenceResult)
	{
		return persistenceResult is not null ? (Error)persistenceResult : Unit.Value;
	}
}