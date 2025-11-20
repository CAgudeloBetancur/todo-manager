using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.Queries.GetTodoById;

public record GetTodoByIdQuery(Guid TodoId) : IRequest< ErrorOr<GetTodoByIdResult>>;