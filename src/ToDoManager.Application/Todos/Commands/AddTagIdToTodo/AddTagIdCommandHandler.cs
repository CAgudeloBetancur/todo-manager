using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Todos;

namespace ToDoManager.Application.Todos.Commands.AddTagIdToTodo;

public class AddTagIdCommandHandler : IRequestHandler<AddTagIdToTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly ITagRepository _tagRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserAccessor _userAccessor;

	public AddTagIdCommandHandler(
		ITodoRepository todoRepository, 
		ITagRepository tagRepository, 
		IUnitOfWork unitOfWork, 
		IUserAccessor userAccessor
		)
	{
		_todoRepository = todoRepository;
		_tagRepository = tagRepository;
		_unitOfWork = unitOfWork;
		_userAccessor = userAccessor;
	}
	
	public async Task<ErrorOr<Unit>> Handle(AddTagIdToTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();
		
		var todoQueryResult = await _todoRepository.GetByIdForUserAsync(request.TodoId, currentUserId);
		var todo = EnsureTodoExists(todoQueryResult);

		if (todo.IsError) return todo.Errors;
		
		var tagQueryResult = await _tagRepository.GetByIdAsync(request.TagId);
		var tag = EnsureTagExists(tagQueryResult);

		if (tag.IsError) return tag.Errors;
		
		todo.Value.AddTagId(tag.Value.Id);
		
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}

	private static ErrorOr<Todo> EnsureTodoExists(Todo? todo)
	{
		return todo is null
			? Errors.ToDo.NotFound
			: todo;
	}

	private static ErrorOr<Tag> EnsureTagExists(Tag? tag)
	{
		return tag is null
			? Errors.Tag.NotFound
			: tag;
	}
}