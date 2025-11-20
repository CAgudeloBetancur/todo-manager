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
		var userId = _userAccessor.GetId();

		if (userId is null) return Errors.Authentication.UserIdNotFound;
		
		var ownerId = UserId.Create((Guid)userId);
		
		var currentTodo = await _todoRepository.GetByIdForUserAsync(
			TodoId.Create(request.TodoId),
			ownerId
			);

		if (currentTodo is null) return Errors.ToDo.NotFound;

		var updatedTodo = Todo.Create(
			currentTodo.Id,
			request.Title,
			request.Description,
			TodoStatus.From(request.Status),
			TodoPriority.Create(request.Priority),
			DueDate.Create(request.DueDate),
			ownerId,
			currentTodo.AuditInfo
		);

		var hasChanges =
			currentTodo.Title != updatedTodo.Title ||
			currentTodo.Description != updatedTodo.Description ||
			currentTodo.Status != updatedTodo.Status ||
			currentTodo.Priority != updatedTodo.Priority ||
			currentTodo.DueDate != updatedTodo.DueDate;

		if (hasChanges)
		{
			updatedTodo.AuditInfo.Update(ownerId.Value, DateTime.UtcNow);

			await _todoRepository.Update(currentTodo, updatedTodo);
			
			var errors = await _unitOfWork.SaveChangesAsync(cancellationToken);
			
			if(errors is not null) return (Error)errors;
		}
		
		return Unit.Value;
	}
};