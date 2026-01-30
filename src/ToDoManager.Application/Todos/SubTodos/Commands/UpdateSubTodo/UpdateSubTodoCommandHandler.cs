using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.SubTodos.Commands.UpdateSubTodo;

public class UpdateSubTodoCommandHandler : IRequestHandler<UpdateSubTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserAccessor _userAccessor;

	public UpdateSubTodoCommandHandler(ITodoRepository todoRepository, IUnitOfWork unitOfWork, IUserAccessor userAccessor)
	{
		_todoRepository = todoRepository;
		_unitOfWork = unitOfWork;
		_userAccessor = userAccessor;
	}
	
	public async Task<ErrorOr<Unit>> Handle(
		UpdateSubTodoCommand request, 
		CancellationToken cancellationToken
		)
	{
		var currentUserId = _userAccessor.GetId();

		var todoId = ToTodoId(request.TodoId);

		var todoResult = await GetTodoByIdForUser(todoId, currentUserId);

		if (todoResult.IsError) return todoResult.Errors;
		
		var todo = todoResult.Value;
		
		var subTodoId = ToSubTodoId(request.SubTodoId);
		
		todo.UpdateSubTodo(
			subTodoId, 
			request.Title, 
			request.Description, 
			request.IsComplete
			);
		
		var persistenceResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

		return EnsurePersistenceSucceeded(persistenceResult);
	}

	private ErrorOr<Unit> EnsurePersistenceSucceeded(Error? persistenceResult)
	{
		return persistenceResult is not null ? (Error)persistenceResult : Unit.Value;
	}

	private TodoId ToTodoId(Guid todoIdFromRequest)
	{
		return TodoId.Create(todoIdFromRequest);
	}
	
	private SubTodoId ToSubTodoId(Guid subTodoIdFromRequest)
	{
		return SubTodoId.Create(subTodoIdFromRequest);
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUser(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);
		return todo is null ? Errors.ToDo.NotFound : todo;
	}
}