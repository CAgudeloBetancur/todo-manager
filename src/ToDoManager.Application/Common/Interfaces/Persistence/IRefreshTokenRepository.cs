using ToDoManager.Application.Common.Interfaces.Persistence.DTOs;

namespace ToDoManager.Application.Common.Interfaces.Persistence;

public interface IRefreshTokenRepository
{
	Task<RefreshTokenDto?> GetByTokenAsync(string token);
	Task AddAsync(RefreshTokenDto refreshToken);
	Task InvalidateAsync(string token);
	Task <IEnumerable<RefreshTokenDto>> GetByUserIdAsync(Guid userId);
}