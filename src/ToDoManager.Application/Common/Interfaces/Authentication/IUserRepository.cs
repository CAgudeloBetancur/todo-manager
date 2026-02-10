using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Common.Interfaces.Authentication;

public interface IUserRepository
{
	Task<User?> FindByEmailAsync(string email);
	Task<User?> FindByIdAsync(UserId email);
	Task<User?> FindByIdWithTodosAsync(UserId userId);
	Task<AuthenticationOperationResult> AddAsync(User user, string password);
	Task<bool> CheckPasswordAsync(User user, string password);
	Task<IList<string>> GetRolesAsync(User user);
	Task<AuthenticationOperationResult> AddToRoleAsync(User user, string roleName);
}
