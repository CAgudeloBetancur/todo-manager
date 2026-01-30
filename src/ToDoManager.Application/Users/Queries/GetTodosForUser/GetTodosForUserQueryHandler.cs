using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos.Entities;
using ToDoManager.Domain.Users;

namespace ToDoManager.Application.Users.Queries.GetTodosForUser;

public class GetTodosForUserQueryHandler : IRequestHandler<GetTodosForUserQuery,  ErrorOr<List<GetTodosForUserResult>>>
{
	private readonly IUserRepository _userRepository;
	private readonly IUserAccessor _userAccessor;

	public GetTodosForUserQueryHandler(IUserAccessor userAccessor, IUserRepository userRepository)
	{
		_userAccessor = userAccessor;
		_userRepository = userRepository;
	}
	
	public async Task<ErrorOr<List<GetTodosForUserResult>>> Handle(GetTodosForUserQuery request, CancellationToken cancellationToken)
	{
		var userWithTodosResult = await FindUserByIdWithTodos(request.UserId);
		
		if(userWithTodosResult.IsError) return userWithTodosResult.Errors;

		var userWithTodos = userWithTodosResult.Value;

		return MapUserTodosToResult(userWithTodos);
	}

	private List<GetTodosForUserResult> MapUserTodosToResult(User user)
	{
		return user
			.Todos
			.Select(
				t => new GetTodosForUserResult(
					t.Id.Value, 
					t.Title, 
					t.Description, 
					t.Status.Value, 
					t.Priority.Label, 
					t.Priority.Value, 
					t.DueDate.Value, 
					t.OwnerId.Value, 
					t.AuditInfo.CreatedAt,
					MapSubTodosToResponse(t.SubTodos),
					MapTagIdsToResponse(t.TagIds)
				)
			)
			.ToList();
	}

	private List<SubTodoResponse> MapSubTodosToResponse(IReadOnlyList<SubTodo> subTodos)
	{
		return subTodos
			.Select(st => new SubTodoResponse(st.Id.Value, st.Title, st.Description))
			.ToList();
	}

	private List<Guid> MapTagIdsToResponse(IReadOnlyList<TagId> tagIds)
	{
		return tagIds
			.Select(tid => tid.Value)
			.ToList();
	}

	private async Task<ErrorOr<User>> FindUserByIdWithTodos(Guid userId)
	{
		var userWithTodos = await _userRepository.FindByIdWithTodosAsync(userId);
		return userWithTodos is null ? Errors.User.NotFound : userWithTodos;
	}
}