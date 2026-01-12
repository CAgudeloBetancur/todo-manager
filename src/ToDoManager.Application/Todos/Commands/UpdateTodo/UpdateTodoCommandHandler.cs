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
		var ownerId = _userAccessor.GetId();
		
		var todo = await _todoRepository.GetByIdForUserAsync(
			TodoId.Create(request.TodoId),
			ownerId
			);

		if (todo is null) return Errors.ToDo.NotFound;
		
		var hasChanges =
			todo.Title != request.Title ||
			todo.Description != request.Description ||
			todo.Status.Value != request.Status ||
			todo.Priority.Value != request.Priority ||
			todo.DueDate.Value != request.DueDate;

		if (hasChanges)
		{
			todo.UpdateTitle(request.Title);
			todo.UpdateDescription(request.Description);
			todo.UpdateStatus(TodoStatus.From(request.Status));
			todo.UpdatePriority(TodoPriority.Create(request.Priority));
			todo.UpdateDueDate(DueDate.Create(request.DueDate));

			await _todoRepository.Update(todo);
			
			var errors = await _unitOfWork.SaveChangesAsync(cancellationToken);
			
			if(errors is not null) return (Error)errors;
		}
		
		return Unit.Value;
	}
};