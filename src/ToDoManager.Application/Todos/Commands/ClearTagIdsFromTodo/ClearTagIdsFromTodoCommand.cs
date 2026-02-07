using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.ClearTagIdsFromTodo;

public record ClearTagIdsFromTodoCommand(Guid TodoId)
	: ICommand<ErrorOr<Unit>>;