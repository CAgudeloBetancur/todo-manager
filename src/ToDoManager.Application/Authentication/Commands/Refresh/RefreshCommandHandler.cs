using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Common.Behaviors;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.DTOs;
using ToDoManager.Application.Common.Interfaces.Services;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Authentication.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, ErrorOr<AuthenticationResult>>
{
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly IUserRepository _userRepository;

	public RefreshCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUserAccessor userAccessor, IDateTimeProvider dateTimeProvider, IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
	{
		_refreshTokenRepository = refreshTokenRepository;
		_dateTimeProvider = dateTimeProvider;
		_userRepository = userRepository;
		_jwtTokenGenerator = jwtTokenGenerator;
	}

	public async Task<ErrorOr<AuthenticationResult>> Handle(RefreshCommand request, CancellationToken cancellationToken)
	{
		var refreshTokenResult = await EnsureRefreshTokenExists(request.RefreshToken);
		if(refreshTokenResult.IsError) return refreshTokenResult.Errors;
		var refreshToken = refreshTokenResult.Value;

		var userId = UserId.Create(refreshToken.UserId);
		var userResult = await EnsureUserExists(userId);
		if(userResult.IsError) return userResult.Errors;
		var user = userResult.Value;
		
		var roles = await _userRepository.GetRolesAsync(user);
		
		var jwt = _jwtTokenGenerator.GenerateToken(user, roles);

		return new AuthenticationResult(jwt, refreshToken.Token);
	}

	private async Task<ErrorOr<User>> EnsureUserExists(UserId userId)
	{
		var user = await _userRepository.FindByIdAsync(userId);
		
		return user is null
			? Errors.User.NotFound
			: user;
	}

	private async Task<ErrorOr<RefreshTokenDto>> EnsureRefreshTokenExists(string requestRefreshToken)
	{
		var refreshToken = await _refreshTokenRepository.GetByTokenAsync(requestRefreshToken);

		return RefreshTokenIsValid(refreshToken)
			? refreshToken
			: Errors.Authentication.InvalidRefreshToken;
	}

	private bool RefreshTokenIsValid(RefreshTokenDto? refreshToken)
	{
		return refreshToken is not null 
			&& !refreshToken.IsRevoked
			&& refreshToken.ExpiresAt > _dateTimeProvider.UtcNow;
	}
}