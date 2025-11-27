using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.SubTodos.Commands.UpdateSubTodo;

public class UpdateSubTodoCommandHandler : IRequestHandler<UpdateSubTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserAccessor _userAccessor;

	public UpdateSubTodoCommandHandler(ITodoRepository todoRepository, IUnitOfWork unitOfWork, IUserAccessor userAccessor)
	{
		_todoRepository = todoRepository;
		_unitOfWork = unitOfWork;
		_userAccessor = userAccessor;
	}
	
	public async Task<ErrorOr<Unit>> Handle(UpdateSubTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();

		if (currentUserId is null) return Errors.User.NotFound;

		var todo = await _todoRepository
			.GetByIdForUserAsync(TodoId.Create(request.TodoId), UserId.Create((Guid)currentUserId));

		if (todo is null) return Errors.ToDo.NotFound;
		
		todo
			.UpdateSubTodo(
				SubTodoId.Create(request.SubTodoId), 
				request.Title, 
				request.Description, 
				request.IsComplete
				);
		
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}