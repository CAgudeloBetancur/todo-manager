using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Domain.Users;

namespace ToDoManager.Application.Authentication.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<Unit>>
{
	private readonly IUserRepository _userRepository;

	public UpdateUserCommandHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<ErrorOr<Unit>> Handle(
		UpdateUserCommand request, 
		CancellationToken cancellationToken
		)
	{
		var domainUser = CreateUserFromRequest(
			request.UserId, 
			request.DisplayName, 
			request.Email, 
			request.FirstName, 
			request.LastName
			);
		
		var updateUserResult = await _userRepository.UpdateUserAsync(domainUser);
		
		if(!updateUserResult.Succeeded) 
			return MapToValidationErrors(updateUserResult.Errors);

		return Unit.Value;
	}

	private User CreateUserFromRequest(
		Guid requestUserId,
		string requestDisplayName, 
		string requestEmail, 
		string requestFirstName, 
		string requestLastName
		)
	{
		return User
			.Create(requestUserId, requestDisplayName, requestEmail, requestFirstName, requestLastName);
	}
	
	private static List<Error> MapToValidationErrors(IEnumerable<AuthenticationError> errors)
	{
		return errors
			.Select(e => Error.Validation(e.Code, e.Description))
			.ToList();
	}
}