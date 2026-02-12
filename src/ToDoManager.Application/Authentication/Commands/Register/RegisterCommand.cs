using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Interfaces;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Authentication.Commands.Register;

public record RegisterCommand(
	string FirstName, 
	string LastName, 
	string Email, 
	string Password
	) : ICommand< ErrorOr<Unit> >, IAllowAnonymous;