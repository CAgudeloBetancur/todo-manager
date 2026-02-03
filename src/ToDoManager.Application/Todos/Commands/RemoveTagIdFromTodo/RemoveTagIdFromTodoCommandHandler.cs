using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.RemoveTagIdFromTodo;

public class RemoveTagIdFromTodoCommandHandler : IRequestHandler<RemoveTagIdFromTodoCommand, ErrorOr<Unit>>
{
	private readonly IUserAccessor _userAccessor;
	private readonly ITodoRepository _todoRepository;
	private readonly IUnitOfWork _unitOfWork;

	public RemoveTagIdFromTodoCommandHandler(IUserAccessor userAccessor, ITodoRepository todoRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork)
	{
		_userAccessor = userAccessor;
		_todoRepository = todoRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<ErrorOr<Unit>> Handle(RemoveTagIdFromTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();

		var todoId = ToTodoId(request.TodoId);

		var queryResult = await GetTodoByIdForUserAsync(todoId, currentUserId);

		if (queryResult.IsError) return queryResult.Errors;

		var todo = queryResult.Value;

		var tagId = ToTagId(request.TagId);
		
		todo.RemoveTagId(tagId);

		var persistenceResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

		return EnsurePersistenceResult(persistenceResult);
	}
	
	private static TodoId ToTodoId(Guid primitiveTodoId) => TodoId.Create(primitiveTodoId);
	private static TagId ToTagId(Guid primitiveTagId) => TagId.Create(primitiveTagId);

	private async Task<ErrorOr<Todo>> GetTodoByIdForUserAsync(TodoId todoId, UserId userId)
	{
		var todo = await _todoRepository.GetByIdForUserAsync(todoId, userId);

		return todo is null ? Errors.ToDo.NotFound : todo;
	}

	private static ErrorOr<Unit> EnsurePersistenceResult(Error? persistenceResult)
	{
		return persistenceResult is not null ? (Error)persistenceResult : Unit.Value;
	}
}