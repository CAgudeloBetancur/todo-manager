using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.RemoveTagIdFromTodo;

public record RemoveTagIdFromTodoCommand(Guid TodoId, Guid TagId) 
	: ICommand<ErrorOr<Unit>>;