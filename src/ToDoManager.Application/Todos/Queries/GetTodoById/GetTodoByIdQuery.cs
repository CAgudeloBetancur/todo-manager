using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Domain.Todos.ValueObjects;

namespace ToDoManager.Application.Todos.Queries.GetTodoById;

public record GetTodoByIdQuery(Guid TodoId) : IQuery< ErrorOr<GetTodoByIdResult>>;