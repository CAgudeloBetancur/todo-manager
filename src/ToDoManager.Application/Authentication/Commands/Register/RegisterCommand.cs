using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Interfaces;

namespace ToDoManager.Application.Authentication.Commands.Register;

public record RegisterCommand(
	string FirstName, 
	string LastName, 
	string Email, 
	string Password
	) : IRequest< ErrorOr<AuthenticationResult> >, IAllowAnonymous;