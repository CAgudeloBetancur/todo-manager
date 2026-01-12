using ErrorOr;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Common.Interfaces.Http;

public interface IUserAccessor
{
	UserId GetId();
	string? GetEmail();
	string? GetUsername();
	List<string>? GetRoles();
	string? GetClaim(string claimType);
	bool? IsInRole(string roleName);
	bool IsUserLoggedIn();

}