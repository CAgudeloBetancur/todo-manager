using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDoManager.Application.Authentication.Common.Interfaces;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Http;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;
using ToDoManager.Application.Common.Interfaces.Services;
using ToDoManager.Infrastructure.Authentication.Adapters;
using ToDoManager.Infrastructure.Authentication.Jwt;
using ToDoManager.Infrastructure.Data.Persistence;
using ToDoManager.Infrastructure.Data.Persistence.Repositories;
using ToDoManager.Infrastructure.Http;
using ToDoManager.Infrastructure.Services;

namespace ToDoManager.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		services
			.AddAuth(configuration)
			.AddPersistence(configuration)
			.AddSingleton<IDateTimeProvider, DateTimeProvider>()
			.AddScoped<IUserAccessor, UserAccessor>();
		
		return services;
	}

	private static IServiceCollection AddPersistence(
		this IServiceCollection services,
		IConfiguration configuration
		)
	{
		var connectionString = configuration.GetConnectionString("DefaultConnection");

		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseNpgsql(
				connectionString,
				npgsqlOptionsAction: npgOptions =>
				{
					npgOptions.EnableRetryOnFailure(
						maxRetryCount: 2,
						maxRetryDelay: TimeSpan.FromSeconds(3),
						null
					);
				})
		);

		services.AddScoped<IUnitOfWork, UnitOfWork>();
		
		services.AddScoped<ITagRepository, TagRepository>();
		services.AddScoped<ITodoRepository, TodoRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

		return services;
	}

	private static IServiceCollection AddAuth(
		this IServiceCollection services, 
		IConfiguration configuration
		)
	{
		var jwtSettings = new JwtSettings();

		configuration.Bind(JwtSettings.SectionName, jwtSettings);
		services.AddSingleton(Options.Create(jwtSettings));
		
		services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings.Issuer,
				ValidAudience = jwtSettings.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
			});

		services
			.AddScoped<IUserManagerAdapter, UserManagerAdapter>();

		return services;
	}
}
