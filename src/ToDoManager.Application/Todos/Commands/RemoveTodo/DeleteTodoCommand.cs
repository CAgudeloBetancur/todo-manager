using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.RemoveTodo;

public record DeleteTodoCommand(Guid TodoId) : ICommand<ErrorOr<Unit>>;