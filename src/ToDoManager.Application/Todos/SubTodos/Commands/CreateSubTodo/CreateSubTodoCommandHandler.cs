using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Domain.Todos.Entities;
using ToDoManager.Domain.Todos.ValueObjects;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Todos.SubTodos.Commands.CreateSubTodo;

public class CreateSubTodoCommandHandler : IRequestHandler<CreateSubTodoCommand,ErrorOr<Unit>>
{
	private readonly ITodoRepository _todoRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IUnitOfWork _unitOfwork;

	public CreateSubTodoCommandHandler(ITodoRepository todoRepository, IUserAccessor userAccessor, IUnitOfWork unitOfwork)
	{
		_todoRepository = todoRepository;
		_userAccessor = userAccessor;
		_unitOfwork = unitOfwork;
	}

	public async Task<ErrorOr<Unit>> Handle(CreateSubTodoCommand request, CancellationToken cancellationToken)
	{
		var currentUserId = _userAccessor.GetId();
		
		var todo = await _todoRepository.GetByIdForUserAsync(TodoId.Create(request.TodoId), currentUserId);
		
		if(todo is null) return Errors.ToDo.NotFound;
		
		todo.AddSubTodo(request.Title, request.Description, request.IsComplete);

		await _todoRepository.Update(todo);
		await _unitOfwork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}