using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Infrastructure.Http;

public sealed class UserAccessor : IUserAccessor
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public UserAccessor(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}
	
	private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

	public UserId GetId()
	{
		var userIdClaimAsString = GetExistingUserIdClaim();
		var userIdAsGuid = ParseUserIdClaim(userIdClaimAsString);
		return UserId.Create(userIdAsGuid);
	}

	private string GetExistingUserIdClaim()
	{
		if(!IsUserLoggedIn()) 
			throw new UnauthorizedAccessException("User not authenticated.");
		
		var claimValue = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		
		if(string.IsNullOrEmpty(claimValue))
			throw new UnauthorizedAccessException("User id missing.");

		return claimValue;
	}

	private static Guid ParseUserIdClaim(string claimValue)
	{
		return !Guid.TryParse(claimValue, out var parsedUserId) 
			? throw new InvalidOperationException("Invalid user id.") 
			: parsedUserId;
	}

	public bool IsUserLoggedIn()
	{
		return (
			User is { Identity.IsAuthenticated: true } && 
			User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier)
			);
	}

	public string? GetEmail()
	{
		return User?.FindFirst(ClaimTypes.Email)?.Value;
	}

	public string? GetUsername()
	{
		return User?.FindFirst(ClaimTypes.Name)?.Value;
	}

	public List<string>? GetRoles()
	{
		return User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
	}

	public string? GetClaim(string claimType)
	{
		return User?.FindFirst(claimType)?.Value;
	}

	public bool? IsInRole(string roleName)
	{
		return GetRoles()?.Contains(roleName);
	}
}