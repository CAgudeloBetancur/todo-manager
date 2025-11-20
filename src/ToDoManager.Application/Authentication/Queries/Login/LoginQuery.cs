using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;

namespace ToDoManager.Application.Authentication.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest< ErrorOr<AuthenticationResult> >;