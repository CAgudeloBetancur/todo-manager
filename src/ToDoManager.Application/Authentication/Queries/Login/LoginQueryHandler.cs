using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
	private readonly IUserRepository _userRepository;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;

	public LoginQueryHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
	{
		_userRepository = userRepository;
		_jwtTokenGenerator = jwtTokenGenerator;
	}

	public async Task< ErrorOr<AuthenticationResult> > Handle(LoginQuery request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.FindByEmailAsync(request.Email);

		if (user is null) return Errors.Authentication.InvalidCredentials;
		
		var passwordCheckResult = await _userRepository.CheckPasswordAsync(user, request.Password);
		
		if (!passwordCheckResult) return Errors.Authentication.InvalidCredentials;

		var userRoles = await _userRepository.GetRolesAsync(user);
		
		var token = _jwtTokenGenerator.GenerateToken(user, userRoles);

		return new AuthenticationResult(
			user,
			token
			);
	}
}