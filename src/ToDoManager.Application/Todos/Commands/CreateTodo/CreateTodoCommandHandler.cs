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

	public async Task<ErrorOr<CreateTodoResult>> Handle(
		CreateTodoCommand request, 
		CancellationToken cancellationToken
		)
	{
		var ownerId = _userAccessor.GetId();

		var todo = BuildTodoFromRequest(request, ownerId);

		await _todoRepository.AddAsync(todo);

		return MapToResult(todo);
	}

	private static Todo BuildTodoFromRequest(CreateTodoCommand request, UserId ownerId)
	{
		return Todo.Create(
			request.Title,
			request.Description ?? string.Empty,
			request.Status,
			request.Priority,
			request.DueDate,
			ownerId
		);
	}

	private static CreateTodoResult MapToResult(Todo todo)
	{
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