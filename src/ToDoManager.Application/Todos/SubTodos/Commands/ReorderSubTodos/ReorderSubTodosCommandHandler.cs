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
		var todoId = TodoId.Create(request.TodoId);
		var todoResult = await GetTodoByIdForUser(todoId, currentUserId);
		
		if (todoResult.IsError) return todoResult.Errors;

		var domainOrderList = ToDomainOrderList(request.SubTodos);
		var todo = todoResult.Value;
		todo.ReorderSubTodos(domainOrderList);
		
		var persistenceResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

		return EnsurePersistenceSucceeded(persistenceResult);
	}

	private static ErrorOr<Unit> EnsurePersistenceSucceeded(Error? persistenceResult)
	{
		return persistenceResult is not null 
			? (Error)persistenceResult 
			: Unit.Value;
	}

	private async Task<ErrorOr<Todo>> GetTodoByIdForUser(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);
		return todo is null 
			? Errors.ToDo.NotFound 
			: todo;
	}

	private static List<(Guid Id, int Order)> ToDomainOrderList(List<ReorderSubTodosDto> subTodosDto)
	{
		return subTodosDto
			.Select(x => (x.Id, x.Order))
			.ToList();
	}
}