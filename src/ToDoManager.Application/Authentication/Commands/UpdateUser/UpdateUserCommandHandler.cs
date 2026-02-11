using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Behaviors;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;

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
		var userId = UserId.Create(request.UserId);
		
		var userResult = await _userRepository.FindByIdAsync(userId);

		var user = EnsureUserExists(userResult);

		if (user.IsError) return user.Errors;
		
		var updateUserResult = await _userRepository.UpdateUserAsync(user.Value);
		
		if(!updateUserResult.Succeeded) 
			return MapToValidationErrors(updateUserResult.Errors);

		return Unit.Value;
	}

	private ErrorOr<User> EnsureUserExists(User? userResult)
	{
		return userResult is null ? Errors.User.NotFound : userResult;
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