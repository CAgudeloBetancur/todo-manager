using ErrorOr;

namespace ToDoManager.Application.Common.Interfaces.Http;

public interface IUserAccessor
{
	Guid? GetId();
	string? GetEmail();
	string? GetUsername();
	List<string>? GetRoles();
	string? GetClaim(string claimType);
	bool? IsInRole(string roleName);
}