using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ToDoManager.Application.Common.Interfaces.Http;

namespace ToDoManager.Infrastructure.Http;

public sealed class UserAccessor : IUserAccessor
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public UserAccessor(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}
	
	private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

	public Guid? GetId()
	{
		var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		
		return Guid.TryParse(userId, out var guid) ? guid : null;
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