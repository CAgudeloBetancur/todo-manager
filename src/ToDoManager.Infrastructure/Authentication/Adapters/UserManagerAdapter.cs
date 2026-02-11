using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoManager.Application.Authentication.Common.Interfaces;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;
using ToDoManager.Infrastructure.Authentication.Identity.Entities;
using ToDoManager.Infrastructure.Data.Persistence;

namespace ToDoManager.Infrastructure.Authentication.Adapters;

public class UserManagerAdapter : IUserManagerAdapter
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly ApplicationDbContext _context;

	public UserManagerAdapter(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
	{
		_userManager = userManager;
		_context = context;
	}

	public async Task<User?> FindByEmailAsync(string email)
	{
		var identityUser = await _userManager.FindByEmailAsync(email);

		return BuildFindUserResult(identityUser);
	}

	public async Task<User?> FindByIdAsync(UserId id)
	{
		var identityUser = await _userManager.FindByIdAsync(id.Value.ToString()); 
		
		return BuildFindUserResult(identityUser);
	}

	public async Task<User?> FindByIdWithTodosAsync(UserId userId)
	{ 
		var identityUser = await _context
			.Users
			.Include(u => u.Todos)
			.ThenInclude(t => t.SubTodos)
			.FirstOrDefaultAsync(u => u.Id == userId.Value);
		
		return BuildFindByIdWithTodosResult(identityUser);
	}
	
	private static User? BuildFindByIdWithTodosResult(ApplicationUser? identityUser)
	{
		return identityUser is null
			? null
			: BuildFindUserResult(identityUser);
	}

	private static User? BuildFindUserResult(ApplicationUser? identityUser)
	{
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
		var identityUser = ToIdentityUser(user);

		var result = await _userManager.CreateAsync(identityUser, password);

		return ToOperationResult(result);
	}

	public async Task<bool> CheckPasswordAsync(User user, string password)
	{
		var identityUser = await _userManager.FindByIdAsync(user.Id.Value.ToString());

		return await BuildCheckPasswordResult(identityUser, password);
	}

	private async Task<bool> BuildCheckPasswordResult(ApplicationUser? identityUser, string password)
	{
		if (identityUser is null) return false;
		
		var passwordIsCorrect = await _userManager.CheckPasswordAsync(identityUser, password);

		return  passwordIsCorrect;

	}

	public async Task<IList<string>> GetRolesAsync(User user)
	{
		var identityUser = await _userManager.FindByIdAsync(user.Id.Value.ToString());

		return await BuildGetRolesResult(identityUser);
	}

	private async Task<IList<string>> BuildGetRolesResult(ApplicationUser? identityUser)
	{
		if (identityUser is null) return new List<string>();

		return await _userManager.GetRolesAsync(identityUser);
	}

	public async Task<AuthenticationOperationResult> AddToRoleAsync(User user, string roleName)
	{
		var identityUser = await _userManager.FindByEmailAsync(user.Email.Value);

		if (identityUser is null) return BuildAuthenticationErrorResultFromIdentityUserNotFound();
		
		var result = await _userManager.AddToRoleAsync(identityUser,  roleName);
		
		return ToOperationResult(result);
	}

	public async Task<AuthenticationOperationResult> UpdateUserAsync(User user)
	{
		var identityUser = ToIdentityUser(user);
		
		var updateUserResult = await _userManager.UpdateAsync(identityUser);

		return ToOperationResult(updateUserResult);
	}

	public async Task<AuthenticationOperationResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
	{
		var identityUser = ToIdentityUser(user);
		
		var changePasswordResult = await _userManager
			.ChangePasswordAsync(identityUser, currentPassword, newPassword);
		
		return ToOperationResult(changePasswordResult);
	}

	public async Task<AuthenticationOperationResult> ChangeEmailAsync(User user, string newEmail)
	{
		var identityUser = ToIdentityUser(user);
		
		var changeEmailAsync = await _userManager.SetEmailAsync(identityUser, newEmail); 
		
		return ToOperationResult(changeEmailAsync);
	}
	
	private static ApplicationUser ToIdentityUser(User user)
	{
		return new ApplicationUser()
		{
			Id = user.Id.Value,
			UserName = user.Email.Value,
			Email = user.Email.Value,
			FirstName = user.FirstName,
			LastName = user.LastName,
		};
	}

	private static AuthenticationOperationResult ToOperationResult(IdentityResult result)
	{
		if(result.Succeeded) return AuthenticationOperationResult.Success();

		var errors = ToAuthenticationErrors(result.Errors);
		
		return AuthenticationOperationResult.Failure(errors);
	}

	private static AuthenticationOperationResult BuildAuthenticationErrorResultFromIdentityUserNotFound()
	{
		var error = new AuthenticationError($"Identity.User", "User not found");
		return AuthenticationOperationResult.Failure([error]);
	}

	private static List<AuthenticationError> ToAuthenticationErrors(IEnumerable<IdentityError> errors)
	{
		return errors
			.Select(e => new AuthenticationError($"Identity.{e.Code}", e.Description))
			.ToList();
	}
}