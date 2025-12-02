using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Queries.GetTodosByUser;

public class GetTodosByUserQueryHandler : IRequestHandler<GetTodosByUserQuery, ErrorOr<List<GetTodosByUserResult>>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;

	public GetTodosByUserQueryHandler(ITodoRepository todoRepository, IUserAccessor userAccessor)
	{
		_todoRepository = todoRepository;
		_userAccessor = userAccessor;
	}

	public async Task<ErrorOr<List<GetTodosByUserResult>>> Handle(GetTodosByUserQuery request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();

		if (currentUserId is null) return Errors.Authentication.UserIdNotFound;

		// var isAdmin = _userAccessor.IsInRole("Admin");
		var isAdmin = false;

		if (!isAdmin && request.UserId != currentUserId) return Errors.Authentication.ForbiddenAccess;

		var todos = await _todoRepository.ListByUserAsync(UserId.Create( (Guid)currentUserId) );

		return todos
			.Select(todo =>
				new GetTodosByUserResult(
					todo.Id.Value,
					todo.Title,
					todo.Description,
					todo.Status.Value,
					todo.Priority.Label,
					todo.Priority.Value,
					todo.DueDate.Value,
					todo.OwnerId.Value,
					todo.AuditInfo.CreatedAt,
					todo.SubTodos.Select(st => new SubTodoResponse(st.Id.Value, st.Title, st.Description)).ToList(),
					todo.TagIds.Select(t => t.Value).ToList()
				)
			)
			.ToList();
	}
}