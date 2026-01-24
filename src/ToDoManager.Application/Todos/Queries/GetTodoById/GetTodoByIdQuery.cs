using ErrorOr;
using MediatR;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Queries.GetTodoById;

public record GetTodoByIdQuery(TodoId TodoId) : IRequest< ErrorOr<GetTodoByIdResult>>;