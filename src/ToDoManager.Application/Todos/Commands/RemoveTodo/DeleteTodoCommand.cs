using ErrorOr;
using MediatR;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Commands.RemoveTodo;

public record DeleteTodoCommand(TodoId TodoId) : IRequest<ErrorOr<Unit>>;