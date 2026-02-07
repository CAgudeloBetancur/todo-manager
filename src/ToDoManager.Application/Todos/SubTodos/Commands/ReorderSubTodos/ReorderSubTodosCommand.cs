using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Todos.SubTodos.Commands.ReorderSubTodos;

public record ReorderSubTodosCommand(Guid TodoId, List<ReorderSubTodosDto> SubTodos) : ICommand<ErrorOr<Unit>>;