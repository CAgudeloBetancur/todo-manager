using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.RemoveTodo;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteTodoCommandHandler(ITodoRepository todoRepository, IUnitOfWork unitOfWork, IUserAccessor userAccessor)
	{
		_todoRepository = todoRepository;
		_unitOfWork = unitOfWork;
		_userAccessor = userAccessor;
	}

	public async Task<ErrorOr<Unit>> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();

		var todo = await _todoRepository.GetByIdForUserAsync(
			TodoId.Create(request.TodoId),
			UserId.Create((Guid)currentUserId)
			);

		if (todo is null) return Errors.ToDo.NotFound;

		await _todoRepository.Remove(todo);
		
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
};