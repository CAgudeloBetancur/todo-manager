using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Authentication.Commands.UpdateUser;

public record class UpdateUserCommand(
	Guid UserId,
	string DisplayName,
	string Email,
	string FirstName, 
	string LastName
	) : ICommand< ErrorOr<Unit> >;