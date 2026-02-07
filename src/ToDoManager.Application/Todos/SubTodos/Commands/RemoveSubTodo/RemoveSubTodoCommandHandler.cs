using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.SubTodos.Commands.RemoveSubTodo;

public class RemoveSubTodoCommandHandler : IRequestHandler<RemoveSubTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IUnitOfWork _unitOfWork;

	public RemoveSubTodoCommandHandler(ITodoRepository todoRepository, IUserAccessor userAccessor, IUnitOfWork unitOfWork)
	{
		_todoRepository = todoRepository;
		_userAccessor = userAccessor;
		_unitOfWork = unitOfWork;
	}

	public async Task<ErrorOr<Unit>> Handle(RemoveSubTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();

		var todoId = ToTodoId(request.TodoId);
		var todoResult = await GetTodoByIdForUser(todoId, currentUserId);
		if (todoResult.IsError) return todoResult.Errors;

		var todo = todoResult.Value;
		var subTodoId = ToSubTodoId(request.SubTodoId);
		todo.RemoveSubTodo(subTodoId);

		return Unit.Value;
	}

	private SubTodoId ToSubTodoId(Guid requestSubTodoId)
	{
		return SubTodoId.Create(requestSubTodoId);
	}

	private static TodoId ToTodoId(Guid requestTodoId)
	{
		return TodoId.Create(requestTodoId);
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUser(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);
		return todo is null ? Errors.ToDo.NotFound : todo;
	}
}