using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.DTOs;
using ToDoManager.Application.Common.Interfaces.Services;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
	private readonly IUserRepository _userRepository;
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly IDateTimeProvider _dateTimeProvider;

	public LoginQueryHandler(
		IUserRepository userRepository, 
		IJwtTokenGenerator jwtTokenGenerator, 
		IRefreshTokenRepository refreshTokenRepository, 
		IDateTimeProvider dateTimeProvider
		)
	{
		_userRepository = userRepository;
		_jwtTokenGenerator = jwtTokenGenerator;
		_refreshTokenRepository = refreshTokenRepository;
		_dateTimeProvider = dateTimeProvider;
	}

	public async Task< ErrorOr<AuthenticationResult> > Handle(LoginQuery request, CancellationToken cancellationToken)
	{
		var user = await _userRepository.FindByEmailAsync(request.Email);
		if (user is null) return Errors.Authentication.InvalidCredentials;
		
		var passwordCheckResult = await _userRepository.CheckPasswordAsync(user, request.Password);
		if (!passwordCheckResult) return Errors.Authentication.InvalidCredentials;
		
		var existingTokens = await _refreshTokenRepository.GetByUserIdAsync(user.Id.Value);
		_refreshTokenRepository.InvalidateAsync(existingTokens);

		var userRoles = await _userRepository.GetRolesAsync(user);
		
		var jwt = _jwtTokenGenerator.GenerateToken(user, userRoles);

		var refreshTokenDto = ToRefreshTokenDto(user.Id.Value);
		
		await _refreshTokenRepository.AddAsync(refreshTokenDto);

		return new AuthenticationResult(jwt, refreshTokenDto.Token);
	}

	private RefreshTokenDto ToRefreshTokenDto(Guid userId)
	{
		return new RefreshTokenDto(
			Guid.NewGuid().ToString("N"),
			userId,
			_dateTimeProvider.UtcNow.AddDays(7),
			false
		);
	}
}