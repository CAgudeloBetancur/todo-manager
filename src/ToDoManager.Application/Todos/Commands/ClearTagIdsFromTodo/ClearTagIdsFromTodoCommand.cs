using ErrorOr;
using MediatR;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.ClearTagIdsFromTodo;

public record ClearTagIdsFromTodoCommand(Guid TodoId)
	: IRequest<ErrorOr<Unit>>;