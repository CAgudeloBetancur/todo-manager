using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.SubTodos.Commands.ReorderSubTodos;

public record ReorderSubTodosCommand(Guid TodoId, List<ReorderSubTodosDto> SubTodos) : IRequest<ErrorOr<Unit>>;