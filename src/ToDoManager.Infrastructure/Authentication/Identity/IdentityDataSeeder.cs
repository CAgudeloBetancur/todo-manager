using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ToDoManager.Infrastructure.Authentication.Identity.Entities;

namespace ToDoManager.Infrastructure.Authentication.Identity;

public static class IdentityDataSeeder
{
	public static async Task SeedRolesAsync(this IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

		string[] roles = ["Admin", "User"];

		foreach (var role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new IdentityRole<Guid>(role));
			}
		}
	}
	
	public static async Task SeedAdminUserAsync(this IServiceProvider serviceProvider)
	{
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		const string adminEmail = "admin@todo.com";
		var adminUser = await userManager.FindByEmailAsync(adminEmail);

		if (adminUser is null)
		{
			adminUser =  new ApplicationUser
			{
				Id = Guid.NewGuid(),
				UserName = adminEmail, 
				Email = adminEmail , 
				FirstName = adminEmail, 
				LastName = adminEmail
			};
			await userManager.CreateAsync(adminUser, "Admin_2025");
			await userManager.AddToRoleAsync(adminUser, "Admin");
		}
	}
}