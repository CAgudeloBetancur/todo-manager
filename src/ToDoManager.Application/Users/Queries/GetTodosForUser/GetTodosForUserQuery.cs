using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Users.Queries.GetTodosForUser;

public record class GetTodosForUserQuery(Guid UserId) : IQuery<ErrorOr<List<GetTodosForUserResult>>>;