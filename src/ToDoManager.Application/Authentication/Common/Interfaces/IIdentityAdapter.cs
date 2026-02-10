using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Domain.Users;

namespace ToDoManager.Application.Authentication.Common.Interfaces;

public interface IIdentityAdapter
{
	Task<User?> FindByEmailAsync(string email);
	Task<AuthenticationOperationResult> AddAsync(User user, string password);
	Task<bool> CheckPasswordAsync(User user, string password);
	Task<IList<string>> GetRolesAsync(User user);
	Task<AuthenticationOperationResult> AddToRoleAsync(User user, string roleName);
}