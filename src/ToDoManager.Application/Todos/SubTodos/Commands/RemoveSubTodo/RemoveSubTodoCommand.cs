using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.SubTodos.Commands.RemoveSubTodo;

public record RemoveSubTodoCommand(Guid TodoId, Guid SubTodoId) : IRequest<ErrorOr<Unit>>;