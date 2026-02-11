using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Authentication.Commands.ChangeEmail;

public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, ErrorOr<Unit>>
{
	private readonly IUserRepository _userRepository;
	private readonly IUserAccessor _userAccessor;

	public ChangeEmailCommandHandler(IUserRepository userRepository, IUserAccessor userAccessor)
	{
		_userRepository = userRepository;
		_userAccessor = userAccessor;
	}

	public async Task<ErrorOr<Unit>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
	{
		var userId = _userAccessor.GetId();

		var user = await _userRepository.FindByIdAsync(userId);
		
		if (user is null)
			return Errors.User.NotFound;
		
		var changeEmailResult = await _userRepository.ChangeEmailAsync(user, request.NewEmail);
		
		if(!changeEmailResult.Succeeded)
			return MapToValidationErrors(changeEmailResult.Errors);

		return Unit.Value;
	}
	
	private static List<Error> MapToValidationErrors(IEnumerable<AuthenticationError> errors)
	{
		return errors
			.Select(e => Error.Validation(e.Code, e.Description))
			.ToList();
	}
}