using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Interfaces;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Authentication.Queries.Login;

public record LoginQuery(string Email, string Password) 
	: IQuery< ErrorOr<AuthenticationResult> >, IAllowAnonymous;