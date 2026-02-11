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
	private ApplicationUser? _identityUser;

	public UserManagerAdapter(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
	{
		_userManager = userManager;
		_context = context;
		_identityUser = null;
	}

	public async Task<User?> FindByEmailAsync(string email)
	{
		var identityUser = await _userManager.FindByEmailAsync(email);
		
		_identityUser = identityUser;

		return BuildFindUserResult(identityUser);
	}

	public async Task<User?> FindByIdAsync(UserId id)
	{
		var identityUser = await _userManager.FindByIdAsync(id.Value.ToString()); 
		
		_identityUser = identityUser;
		
		return BuildFindUserResult(identityUser);
	}

	public async Task<User?> FindByIdWithTodosAsync(UserId userId)
	{ 
		var identityUser = await _context
			.Users
			.Include(u => u.Todos)
			.ThenInclude(t => t.SubTodos)
			.FirstOrDefaultAsync(u => u.Id == userId.Value);
		
		_identityUser = identityUser;
		
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
		_identityUser = await EnsureValidIdentityUser(user);
		
		return await BuildCheckPasswordResult(_identityUser, password);
	}

	private async Task<bool> BuildCheckPasswordResult(ApplicationUser? identityUser, string password)
	{
		if (identityUser is null) return false;
		
		var passwordIsCorrect = await _userManager.CheckPasswordAsync(identityUser, password);

		return  passwordIsCorrect;

	}

	public async Task<IList<string>> GetRolesAsync(User user)
	{
		_identityUser = await EnsureValidIdentityUser(user);

		return await BuildGetRolesResult(_identityUser);
	}

	private async Task<IList<string>> BuildGetRolesResult(ApplicationUser? identityUser)
	{
		if (identityUser is null) return new List<string>();

		return await _userManager.GetRolesAsync(identityUser);
	}

	public async Task<AuthenticationOperationResult> AddToRoleAsync(User user, string roleName)
	{
		_identityUser = await EnsureValidIdentityUser(user);

		if (_identityUser is null) return BuildAuthenticationErrorResultFromIdentityUserNotFound();
		
		var result = await _userManager.AddToRoleAsync(_identityUser,  roleName);
		
		return ToOperationResult(result);
	}

	public async Task<AuthenticationOperationResult> UpdateUserAsync(User user)
	{
		_identityUser = await EnsureValidIdentityUser(user);
		
		if (_identityUser is null) return BuildAuthenticationErrorResultFromIdentityUserNotFound();

		if (!UserChanged(user.Email.Value, user.FirstName, user.LastName, _identityUser))
			return AuthenticationOperationResult.Success();
		
		_identityUser.Email = user.Email.Value;
		_identityUser.UserName = user.Email.Value;
		_identityUser.FirstName = user.FirstName;
		_identityUser.LastName = user.LastName;
			
		var updateUserResult = await _userManager.UpdateAsync(_identityUser);
		
		return ToOperationResult(updateUserResult);

	}
	
	private static bool UserChanged(string newEmail, string newFirstName, string newLastName, ApplicationUser identityUser)
	{
		return identityUser.Email != newEmail ||
			identityUser.FirstName != newFirstName ||
			identityUser.LastName != newLastName;
	}

	public async Task<AuthenticationOperationResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
	{
		_identityUser = await EnsureValidIdentityUser(user);
		
		if (_identityUser is null) return BuildAuthenticationErrorResultFromIdentityUserNotFound();
		
		var changePasswordResult = await _userManager
			.ChangePasswordAsync(_identityUser, currentPassword, newPassword);
		
		return ToOperationResult(changePasswordResult);
	}

	public async Task<AuthenticationOperationResult> ChangeEmailAsync(User user, string newEmail)
	{
		_identityUser = await EnsureValidIdentityUser(user);
		
		if (_identityUser is null) return BuildAuthenticationErrorResultFromIdentityUserNotFound();
		
		var changeEmailAsync = await _userManager.SetEmailAsync(_identityUser, newEmail); 
		
		return ToOperationResult(changeEmailAsync);
	}
	
	private async Task<ApplicationUser?> EnsureValidIdentityUser(User user)
	{
		if (_identityUser is null || user.Id.Value != _identityUser.Id)
		{
			return await _userManager.FindByIdAsync(user.Id.Value.ToString());
		}

		return _identityUser;
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