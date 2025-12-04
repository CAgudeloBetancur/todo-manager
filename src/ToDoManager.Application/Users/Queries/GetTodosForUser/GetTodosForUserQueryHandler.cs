using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Http;

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
		var userWithTodos = await _userRepository.FindByIdWithTodosAsync(request.UserId);
		
		if(userWithTodos is null) return Errors.User.NotFound;
		
		return userWithTodos
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
					t.SubTodos.Select(st => new SubTodoResponse(st.Id.Value, st.Title, st.Description)).ToList(),
					t.TagIds.Select(tid => tid.Value).ToList()
					)
				)
				.ToList();
	}
}