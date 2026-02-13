namespace ToDoManager.Application.Common.Interfaces.Persistence.DTOs;

public record RefreshTokenDto(
	string Token, 
	Guid UserId, 
	DateTime ExpiresAt, 
	bool IsRevoked
	);