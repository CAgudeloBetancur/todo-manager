using ErrorOr;
using MediatR;
using ToDoManager.Domain.Tags.ValueObjects;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.RemoveTagIdFromTodo;

public record RemoveTagIdFromTodoCommand(Guid TodoId, Guid TagId) : IRequest<ErrorOr<Unit>>;