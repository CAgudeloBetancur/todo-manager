using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Domain.Users;

namespace ToDoManager.Application.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ErrorOr<Unit>>
{
	private readonly IUserAccessor _userAccessor;
	private readonly IUserRepository _userRepository;

	public ChangePasswordCommandHandler(IUserAccessor userAccessor, IUserRepository userRepository)
	{
		_userAccessor = userAccessor;
		_userRepository = userRepository;
	}

	public async Task<ErrorOr<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
	{
		var userId = _userAccessor.GetId();

		var userResult = await _userRepository.FindByIdAsync(userId);

		var user = EnsureUserExist(userResult);

		if (user.IsError) return user.Errors;
		
		var changePasswordResult = await _userRepository
			.ChangePasswordAsync(user.Value, request.CurrentPassword, request.NewPassword);

		if (!changePasswordResult.Succeeded)
			return MapToValidationErrors(changePasswordResult.Errors);
		
		return Unit.Value;
	}

	private ErrorOr<User> EnsureUserExist(User? userResult)
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