using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;
using ToDoManager.Infrastructure.Authentication.Identity.Entities;

namespace ToDoManager.Infrastructure.Data.Persistence.Repositories;

public class UserRepository : IUserRepository
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly ApplicationDbContext _context;

	public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
	{
		_userManager = userManager;
		_context = context;
	}

	public async Task<User?> FindByEmailAsync(string email)
	{
		var identityUser = await _userManager.FindByEmailAsync(email);

		return identityUser is null
			? null
			: User.Create(identityUser.Id, identityUser.Email, identityUser.Email, identityUser.FirstName, identityUser.LastName);
	}

	public async Task<User?> FindByIdWithTodosAsync(Guid userId)
	{
		var identityUser = await _context
			.Users
			.Include(u => u.Todos)
			.ThenInclude(t => t.SubTodos)
			.FirstOrDefaultAsync(u => u.Id == userId);
		
		return identityUser is null
			? null
			: User
				.Create(
					identityUser.Id, 
					identityUser.Email, 
					identityUser.Email, 
					identityUser.FirstName, 
					identityUser.LastName, 
					identityUser.Todos.ToList()
					);
	}

	public async Task<AuthenticationOperationResult> AddAsync(User user, string password)
	{
		var identityUser = new ApplicationUser()
		{
			Id = user.Id.Value,
			UserName = user.Email.Value,
			Email = user.Email.Value,
			FirstName = user.FirstName,
			LastName = user.LastName,
		};

		var result = await _userManager.CreateAsync(identityUser, password);
		
		if(result.Succeeded) return AuthenticationOperationResult.Success();
		
		var errors = result.Errors
			.Select(e => new AuthenticationError($"Identity.{e.Code}", e.Description))
			.ToList();
		
		return AuthenticationOperationResult.Failure(errors);
	}

	public async Task<bool> CheckPasswordAsync(User user, string password)
	{
		var identityUser = await _userManager.FindByIdAsync(user.Id.Value.ToString());	
		
		if (identityUser is null) return false;
		
		return await _userManager.CheckPasswordAsync(identityUser, password);
	}

	public async Task<IList<string>> GetRolesAsync(User user)
	{
		var identityUser = await _userManager.FindByIdAsync(user.Id.Value.ToString());
		
		if (identityUser is null) return new List<string>();
		
		return await _userManager.GetRolesAsync(identityUser);
	}

	public async Task<AuthenticationOperationResult> AddToRoleAsync(User user, string roleName)
	{
		var identityUser = await _userManager.FindByEmailAsync(user.Email.Value);

		if (identityUser is null)
		{
			var error = new AuthenticationError($"Identity.User", "User not found");
			return AuthenticationOperationResult.Failure([error]);
		}
		
		var result = await _userManager.AddToRoleAsync(identityUser,  roleName);
		
		if(result.Succeeded) return AuthenticationOperationResult.Success();
		
		var errors = result.Errors
			.Select(e => new AuthenticationError($"Identity.{e.Code}", e.Description))
			.ToList();
		
		return AuthenticationOperationResult.Failure(errors);
	}
}