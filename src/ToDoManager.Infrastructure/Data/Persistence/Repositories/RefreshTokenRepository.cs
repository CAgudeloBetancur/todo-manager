using Microsoft.EntityFrameworkCore;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.DTOs;
using ToDoManager.Infrastructure.Authentication.Jwt.Models;

namespace ToDoManager.Infrastructure.Data.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
	private readonly ApplicationDbContext _context;

	public RefreshTokenRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<RefreshTokenDto?> GetByTokenAsync(string token)
	{
		var refreshToken = await _context
			.RefreshTokens
			.FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);

		return refreshToken is null 
			? null 
			: ToRefreshTokenDto(refreshToken);
	}

	public async Task AddAsync(RefreshTokenDto refreshTokenDto)
	{
		var refreshToken = ToRefreshToken(refreshTokenDto);
		
		await _context.RefreshTokens.AddAsync(refreshToken);
	}

	public async Task InvalidateAsync(string token)
	{
		var refreshToken = await _context
			.RefreshTokens
			.FirstOrDefaultAsync(rt => rt.Token == token);

		if (refreshToken is not null)
			refreshToken.IsRevoked = true;
	}

	public async Task<IEnumerable<RefreshTokenDto>> GetByUserIdAsync(Guid userId)
	{
		return await _context
			.RefreshTokens
			.Where(rt => rt.UserId == userId && !rt.IsRevoked)
			.Select(rt => ToRefreshTokenDto(rt))
			.ToListAsync();
	}
	
	private static RefreshTokenDto ToRefreshTokenDto(RefreshToken refreshToken)
	{
		return new RefreshTokenDto(
			refreshToken.Token,
			refreshToken.UserId,
			refreshToken.ExpiresAt,
			refreshToken.IsRevoked
		);
	}
	
	private static RefreshToken ToRefreshToken(RefreshTokenDto refreshTokenDto)
	{
		return new RefreshToken
		{
			Token = refreshTokenDto.Token,
			UserId = refreshTokenDto.UserId,
			ExpiresAt = refreshTokenDto.ExpiresAt,
			IsRevoked = refreshTokenDto.IsRevoked
		};
	}
}