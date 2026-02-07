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
	private readonly IUserAccessor _userAccessor;

	public UpdateSubTodoCommandHandler(ITodoRepository todoRepository, IUnitOfWork unitOfWork, IUserAccessor userAccessor)
	{
		_todoRepository = todoRepository;
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

		return Unit.Value;
	}

	private static TodoId ToTodoId(Guid todoIdFromRequest)
	{
		return TodoId.Create(todoIdFromRequest);
	}
	
	private static SubTodoId ToSubTodoId(Guid subTodoIdFromRequest)
	{
		return SubTodoId.Create(subTodoIdFromRequest);
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUser(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);
		return todo is null ? Errors.ToDo.NotFound : todo;
	}
}