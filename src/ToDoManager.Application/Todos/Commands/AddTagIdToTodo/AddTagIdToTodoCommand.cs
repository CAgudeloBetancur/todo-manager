using ErrorOr;
using MediatR;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.AddTagIdToTodo;

public record AddTagIdToTodoCommand(
	Guid TodoId, 
	Guid TagId
	)	: IRequest<ErrorOr<Unit>>;