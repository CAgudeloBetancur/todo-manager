using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Todos.SubTodos.Commands.RemoveSubTodo;

public record RemoveSubTodoCommand(Guid TodoId, Guid SubTodoId) : ICommand<ErrorOr<Unit>>;