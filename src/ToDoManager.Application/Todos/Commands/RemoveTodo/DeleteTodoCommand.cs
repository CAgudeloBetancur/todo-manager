using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.Commands.RemoveTodo;

public record DeleteTodoCommand(Guid TodoId) : IRequest<ErrorOr<Unit>>;