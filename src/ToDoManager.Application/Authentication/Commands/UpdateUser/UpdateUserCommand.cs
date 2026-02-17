using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Common.Interfaces.CQRS;
using Unit = MediatR.Unit;

namespace ToDoManager.Application.Authentication.Commands.UpdateUser;

public record class UpdateUserCommand(
	string Email,
	string FirstName, 
	string LastName
	) : ICommand< ErrorOr<Unit> >;