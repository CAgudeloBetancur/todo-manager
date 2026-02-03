using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.Entities;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Queries.GetTodoById;

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, ErrorOr<GetTodoByIdResult>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;

	public GetTodoByIdQueryHandler(ITodoRepository todoRepository, IUserAccessor userAccessor)
	{
		_todoRepository = todoRepository;
		_userAccessor = userAccessor;
	}

	public async Task<ErrorOr<GetTodoByIdResult>> Handle(
		GetTodoByIdQuery request, 
		CancellationToken cancellationToken
		)
	{
		var currentUserId = _userAccessor.GetId();

		var todoId = ToTodoId(request.TodoId);
		
		var queryResult = await GetTodoByIdForUserAsync(todoId, currentUserId);
		if (queryResult.IsError) return queryResult.Errors;

		var todo = queryResult.Value;
		
		return MapToResult(todo);
	}

	private static TodoId ToTodoId(Guid primitiveTodoId) => TodoId.Create(primitiveTodoId);

	private static ErrorOr<GetTodoByIdResult> MapToResult(Todo todo)
	{
		return new GetTodoByIdResult
		(
			todo.Id.Value,
			todo.Title,
			todo.Description,
			todo.Status.Value,
			todo.Priority.Value,
			todo.Priority.Label,
			todo.DueDate.Value,
			todo.OwnerId.Value,
			todo.AuditInfo.CreatedAt,
			MapSubTodos(todo.SubTodos)
		);
	}

	private static List<SubTodoResponse> MapSubTodos(IEnumerable<SubTodo> subTodos)
	{
		return subTodos
			.OrderBy(st => st.Order)
			.Select(st => new SubTodoResponse(st.Title, st.Description, st.IsComplete, st.Order))
			.ToList();
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUserAsync(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);
		return todo is null ? Errors.ToDo.NotFound : todo;
	}
}