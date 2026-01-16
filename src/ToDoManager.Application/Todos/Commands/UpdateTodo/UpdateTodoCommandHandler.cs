using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Common.ValueObjects;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.UpdateTodo;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateTodoCommandHandler(
		ITodoRepository todoRepository, 
		IUnitOfWork unitOfWork, 
		IUserAccessor userAccessor
		)
	{
		_todoRepository = todoRepository;
		_unitOfWork = unitOfWork;
		_userAccessor = userAccessor;
	}

	public async Task<ErrorOr<Unit>> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
	{
		var ownerId = _userAccessor
			.GetId();

		var todoResult = await GetTodoByIdForUser(request.TodoId, ownerId);
		
		if (todoResult.IsError) 
			return todoResult.Errors;

		var todo = todoResult.Value;

		if (!HasChangesComparedToRequest(todo, request)) 
			return Unit.Value;
		
		ApplyChanges(request, todo);
		
		await _todoRepository.Update(todo);
		
		var persistenceResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
		return EnsurePersistenceSucceeded(persistenceResult);
	}

	private static void ApplyChanges(UpdateTodoCommand request, Todo todo)
	{
		todo.Update(
			request.Title, 
			request.Description, 
			request.Status, 
			request.Priority, 
			request.DueDate
			);
	}

	private static bool HasChangesComparedToRequest(Todo todo, UpdateTodoCommand request)
	{
		return todo.HasChanges(
			request.Title,
			request.Description,
			request.DueDate,
			request.Priority,
			request.Status
			);
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUser(TodoId todoId, UserId ownerId)
	{
		var todo = await _todoRepository
			.GetByIdForUserAsync(todoId, ownerId);

		return todo is null 
			? Errors.ToDo.NotFound 
			: todo;
	}
	
	private static ErrorOr<Unit> EnsurePersistenceSucceeded(Error? persistenceResult)
	{
		return persistenceResult is not null 
			? (Error)persistenceResult 
			: Unit.Value;
	}
};