using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Todos.Queries.GetTodosByUser;

public record GetTodosByUserQuery(Guid UserId) : IRequest< ErrorOr<List<GetTodosByUserResult>>>;