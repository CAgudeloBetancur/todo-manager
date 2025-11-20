using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.Commands.DeleteTodo;

public record DeleteTodoCommand(Guid TodoId) : IRequest<ErrorOr<Unit>>;