using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
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

	public async Task<ErrorOr<GetTodoByIdResult>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();
		
		var todo = await _todoRepository.GetByIdForUserAsync( 
			TodoId.Create(request.TodoId),  
			currentUserId
			);

		if (todo is null) return Errors.ToDo.NotFound;
		
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
			todo
				.SubTodos
				.OrderBy(st => st.Order)
				.Select(st => new SubTodoResponse(st.Title, st.Description, st.IsComplete, st.Order))
				.ToList()
		);
	}
}