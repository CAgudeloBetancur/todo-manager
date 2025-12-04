using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Users.Queries.GetTodosForUser;

public record class GetTodosForUserQuery(Guid UserId) : IRequest<ErrorOr<List<GetTodosForUserResult>>>;