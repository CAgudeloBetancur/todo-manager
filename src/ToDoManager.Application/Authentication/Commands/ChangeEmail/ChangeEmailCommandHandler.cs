using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Authentication.Commands.ChangeEmail;

public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, ErrorOr<Unit>>
{
	private readonly IUserRepository _userRepository;

	public ChangeEmailCommandHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<ErrorOr<Unit>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
	{
		var userId = ToUserId(request.UserId);
		
		var changeEmailResult = await _userRepository.ChangeEmailAsync(userId, request.NewEmail);
		
		if(!changeEmailResult.Succeeded)
			return MapToValidationErrors(changeEmailResult.Errors);

		return Unit.Value;
	}

	private static UserId ToUserId(Guid requestUserId)
	{
		return UserId.Create(requestUserId);
	}
	
	private static List<Error> MapToValidationErrors(IEnumerable<AuthenticationError> errors)
	{
		return errors
			.Select(e => Error.Validation(e.Code, e.Description))
			.ToList();
	}
}