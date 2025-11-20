using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Common.ValueObjects;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.CreateTodo;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, ErrorOr<CreateTodoResult>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IUnitOfWork _unitOfWork;

	public CreateTodoCommandHandler(ITodoRepository todoRepository, IUserAccessor userAccessor, IUnitOfWork unitOfWork)
	{
		_todoRepository = todoRepository;
		_userAccessor = userAccessor;
		_unitOfWork = unitOfWork;
	}

	public async Task<ErrorOr<CreateTodoResult>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();
		
		if(currentUserId is null) return Errors.Authentication.UserIdNotFound;
		
		var ownerId = UserId.Create((Guid)currentUserId);

		var status = request.Status is not null
			? TodoStatus.From(request.Status)
			: TodoStatus.Pending;

		var priority = request.Priority is not null
			? TodoPriority.Create((int)request.Priority)
			: TodoPriority.Medium;

		var dueDate = DueDate.Create(request.DueDate ?? DateTime.UtcNow.AddDays(3));

		var todo = Todo.Create(
			request.Title,
			request.Description ?? string.Empty,
			status,
			priority,
			dueDate,
			ownerId,
			AuditInfo.Create(ownerId.Value, DateTime.UtcNow)
			);

		await _todoRepository.AddAsync(todo);
		
		var error = await _unitOfWork.SaveChangesAsync(cancellationToken);

		if (error is not null) return (Error)error;
		
		return new CreateTodoResult(
			todo.Id.Value,
			todo.Title,
			todo.Description,
			todo.Status.Value,
			todo.Priority.Label,
			todo.DueDate.Value,
			todo.OwnerId.Value,
			todo.AuditInfo.CreatedAt
			);
	}
}