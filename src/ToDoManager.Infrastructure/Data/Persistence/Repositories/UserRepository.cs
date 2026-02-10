using ToDoManager.Application.Authentication.Common.Interfaces;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Infrastructure.Data.Persistence.Repositories;

public class UserRepository : IUserRepository
{
	private readonly IUserManagerAdapter _userManagerAdapter;

	public UserRepository(IUserManagerAdapter userManagerAdapter)
	{
		_userManagerAdapter = userManagerAdapter;
	}

	public async Task<User?> FindByEmailAsync(string email)
	{
		return await _userManagerAdapter.FindByEmailAsync(email);
	}

	public async Task<User?> FindByIdWithTodosAsync(UserId userId)
	{
		return await _userManagerAdapter.FindByIdWithTodosAsync(userId);
	}

	public async Task<User?> FindByIdAsync(UserId userId)
	{
		return await _userManagerAdapter.FindByIdAsync(userId);
	}

	public async Task<AuthenticationOperationResult> AddAsync(User user, string password)
	{
		return await _userManagerAdapter.AddAsync(user, password); 
	}

	public async Task<bool> CheckPasswordAsync(User user, string password)
	{
		return await _userManagerAdapter.CheckPasswordAsync(user, password);
	}

	public async Task<IList<string>> GetRolesAsync(User user)
	{
		return await _userManagerAdapter.GetRolesAsync(user);
	}

	public async Task<AuthenticationOperationResult> AddToRoleAsync(User user, string roleName)
	{
		return await _userManagerAdapter.AddToRoleAsync(user, roleName);
	}
}