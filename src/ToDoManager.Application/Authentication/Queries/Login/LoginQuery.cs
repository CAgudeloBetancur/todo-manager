using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Interfaces;

namespace ToDoManager.Application.Authentication.Queries.Login;

public record LoginQuery(string Email, string Password) 
	: IRequest< ErrorOr<AuthenticationResult> >, IAllowAnonymous;