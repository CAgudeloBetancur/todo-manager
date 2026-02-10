using Microsoft.AspNetCore.Identity;
using ToDoManager.Application.Authentication.Common.Interfaces;
using ToDoManager.Application.Authentication.Common.Persistence;
using ToDoManager.Domain.Users;
using ToDoManager.Infrastructure.Authentication.Identity.Entities;

namespace ToDoManager.Infrastructure.Authentication.Adapters;

public class IdentityAdapter : IIdentityAdapter
{
	private readonly UserManager<ApplicationUser> _userManager;

	public IdentityAdapter(UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task<User?> FindByEmailAsync(string email)
	{
		var identityUser = await _userManager.FindByEmailAsync(email);

		return BuildFindByEmailResult(identityUser);
	}

	private static User? BuildFindByEmailResult(ApplicationUser? identityUser)
	{
		return identityUser is null
			? null
			: User
				.Create(
					identityUser.Id, 
					identityUser.Email, 
					identityUser.Email, 
					identityUser.FirstName, 
					identityUser.LastName
					);
	}

	public async Task<AuthenticationOperationResult> AddAsync(User user, string password)
	{
		var identityUser = ToIdentityUser(user);

		var result = await _userManager.CreateAsync(identityUser, password);

		return ToOperationResult(result);
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