using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Tags;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.AddTagIdToTodo;

public class AddTagIdToTodoCommandHandler : IRequestHandler<AddTagIdToTodoCommand, ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly ITagRepository _tagRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserAccessor _userAccessor;

	public AddTagIdToTodoCommandHandler(
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

		var todoId = ToTodoId(request.TagId);
		
		var todoQueryResult = await _todoRepository.GetByIdForUserAsync(todoId, currentUserId);
		var todo = EnsureTodoExists(todoQueryResult);

		if (todo.IsError) return todo.Errors;

		var tagId = ToTagId(request.TagId);
		
		var tagQueryResult = await _tagRepository.GetByIdAsync(tagId);
		var tag = EnsureTagExists(tagQueryResult);

		if (tag.IsError) return tag.Errors;
		
		todo.Value.AddTagId(tag.Value.Id);
		
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}

	private static TodoId ToTodoId(Guid primitiveTodoId)
	{
		return TodoId.Create(primitiveTodoId);
	}
	
	private static TagId ToTagId(Guid primitiveTagId)
	{
		return TagId.Create(primitiveTagId);
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