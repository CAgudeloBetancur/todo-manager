using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.SubTodos.Commands.ReorderSubTodos;

public class ReorderSubTodosCommandHandler : IRequestHandler<ReorderSubTodosCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserAccessor _userAccessor;

	public ReorderSubTodosCommandHandler(ITodoRepository todoRepository, IUnitOfWork unitOfWork, IUserAccessor userAccessor)
	{
		_todoRepository = todoRepository;
		_unitOfWork = unitOfWork;
		_userAccessor = userAccessor;
	}
	
	public async Task<ErrorOr<Unit>> Handle(ReorderSubTodosCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();
		
		var todo = await _todoRepository
			.GetByIdForUserAsync(TodoId.Create(request.TodoId), currentUserId);

		if (todo is null) return Errors.ToDo.NotFound;

		var domainOrderList = request
			.SubTodos
			.Select(x => (x.Id, x.Order))
			.ToList();
		
		todo.ReorderSubTodos(domainOrderList);
		
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}