using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Behaviors;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;
using Unit = MediatR.Unit;

namespace ToDoManager.Application.Authentication.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<Unit>>
{
	private readonly IUserRepository _userRepository;
	private readonly IUserAccessor _userAccessor;

	public UpdateUserCommandHandler(IUserRepository userRepository, IUserAccessor userAccessor)
	{
		_userRepository = userRepository;
		_userAccessor = userAccessor;
	}

	public async Task<ErrorOr<Unit>> Handle(
		UpdateUserCommand request, 
		CancellationToken cancellationToken
		)
	{
		var userId = _userAccessor.GetId();
		
		var userResult = await _userRepository.FindByIdAsync(userId);

		var user = EnsureUserExists(userResult);

		if (user.IsError) return user.Errors;
		
		user.Value.Update(request.Email, request.Email, request.FirstName, request.LastName);
		
		var updateUserResult = await _userRepository.UpdateUserAsync(user.Value);
		
		if(!updateUserResult.Succeeded) 
			return MapToValidationErrors(updateUserResult.Errors);

		return Unit.Value;
	}

	private ErrorOr<User> EnsureUserExists(User? userResult)
	{
		return userResult is null ? Errors.User.NotFound : userResult;
	}
	
	private static List<Error> MapToValidationErrors(IEnumerable<AuthenticationError> errors)
	{
		return errors
			.Select(e => Error.Validation(e.Code, e.Description))
			.ToList();
	}
}