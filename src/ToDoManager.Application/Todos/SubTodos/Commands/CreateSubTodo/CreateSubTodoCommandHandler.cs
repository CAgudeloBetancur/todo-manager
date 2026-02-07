using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.Entities;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.SubTodos.Commands.CreateSubTodo;

public class CreateSubTodoCommandHandler : IRequestHandler<CreateSubTodoCommand,ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IUnitOfWork _unitOfwork;

	public CreateSubTodoCommandHandler(ITodoRepository todoRepository, IUserAccessor userAccessor, IUnitOfWork unitOfwork)
	{
		_todoRepository = todoRepository;
		_userAccessor = userAccessor;
		_unitOfwork = unitOfwork;
	}

	public async Task<ErrorOr<Unit>> Handle(CreateSubTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();

		var todoId = ToTodoId(request.TodoId);

		var todoResult = await GetTodoByIdForUser(todoId, currentUserId);

		if (todoResult.IsError) return todoResult.Errors;

		var todo = todoResult.Value;
		
		todo.AddSubTodo(request.Title, request.Description, request.IsComplete);

		await _todoRepository.Update(todo);

		return Unit.Value;
	}

	private static TodoId ToTodoId(Guid todoId)
	{
		return TodoId.Create(todoId);
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUser(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);
		
		return todo is null ? Errors.ToDo.NotFound : todo;
	}
}