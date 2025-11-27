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

		if (currentUserId is null) return Errors.User.NotFound;
		
		var todo = await _todoRepository.GetByIdForUserAsync(TodoId.Create(request.TodoId), UserId.Create((Guid)currentUserId));

		if (todo is null) return Errors.ToDo.NotFound;
		
		todo.RemoveSubTodo(SubTodoId.Create(request.SubTodoId));
		
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}