using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.Persistence;

namespace ToDoManager.Application.Authentication.Commands.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, ErrorOr<Unit>>
{
	private readonly IRefreshTokenRepository _refreshTokenRepository;

	public LogoutHandler(IRefreshTokenRepository refreshTokenRepository)
	{
		_refreshTokenRepository = refreshTokenRepository;
	}

	public async Task<ErrorOr<Unit>> Handle(LogoutCommand request,CancellationToken cancellationToken)
	{
		var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

		if (refreshToken is null || refreshToken.IsRevoked)
			return Unit.Value;
		
		await _refreshTokenRepository.InvalidateAsync(refreshToken.Token);
		
		return Unit.Value;
	}
}