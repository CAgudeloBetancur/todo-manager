using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Domain.Users;

namespace ToDoManager.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<Unit>>
{
	private readonly IUserRepository _userRepository;

	public RegisterCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
	{
		_userRepository = userRepository;
	}

	public async Task< ErrorOr<Unit> > Handle(RegisterCommand request, CancellationToken cancellationToken)
	{
		if (await EmailExistsAsync(request.Email)) 
			return Errors.User.DuplicatedEmail;

		var user = CreateUserFromRequest(request.Email, request.Email, request.FirstName, request.LastName);

		var persistenceResult = await PersistUserAsync(user, request.Password);
		
		if(!persistenceResult.Succeeded) 
			return MapToValidationErrors(persistenceResult.Errors);

		var assignToRoleResult = await AssignDefaultRole(user);
		
		if(!assignToRoleResult.Succeeded)
			return MapToValidationErrors(assignToRoleResult.Errors);
		
		return Unit.Value;
	}

	private async Task<AuthenticationOperationResult> AssignDefaultRole(User user)
	{
		return await _userRepository.AddToRoleAsync(user, "User");
	}

	private static User CreateUserFromRequest(string displayName, string email, string firstName, string lastName)
	{
		return User.Create(displayName, email, firstName, lastName);
	}

	private async Task<bool> EmailExistsAsync(string email)
	{
		return await _userRepository.FindByEmailAsync(email) is not null;
	}

	private async Task<AuthenticationOperationResult> PersistUserAsync(User user, string password)
	{
		return await _userRepository.AddAsync(user, password);
	}

	private static List<Error> MapToValidationErrors(IEnumerable<AuthenticationError> errors)
	{
		return errors
			.Select(e => Error.Validation(e.Code, e.Description))
			.ToList();
	}
}