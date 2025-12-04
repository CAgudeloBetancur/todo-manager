using System.Diagnostics;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi.Models;
using ToDoManager.Api.Common.Errors;
using ToDoManager.Api.Common.Http;
using ToDoManager.Api.OpenApi;

namespace ToDoManager.Api;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentation(this IServiceCollection services)
	{
		services.AddProblemDetails(configure =>
		{
			configure.CustomizeProblemDetails = context =>
			{
				var httpContext = context.HttpContext;
				var problemDetails = context.ProblemDetails;
				
				var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

				if (!problemDetails.Extensions.ContainsKey("traceId"))
				{
					problemDetails.Extensions.Add("traceId", traceId);
				}
				
				problemDetails.Status ??= httpContext.Response.StatusCode;
			};
		});
			
		services.AddExceptionHandler<GlobalExceptionHandler>();
		
		services.AddControllers();
		
		services.AddSingleton<ProblemDetailsFactory, ToDoManagerProblemDetailsFactory>();
		
		services.AddEndpointsApiExplorer();
		
		services
			.ConfigureOptions<ConfigureSwaggerGenOptions>();
		
		services.AddSwaggerGen(c =>
		{
			var jwtSecurityScheme = new OpenApiSecurityScheme
			{
				Scheme = "bearer",
				BearerFormat = "JWT",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.Http,
				Description = "Introduce JWT token in this field",
				Reference = new OpenApiReference
				{
					Id = JwtBearerDefaults.AuthenticationScheme,
					Type = ReferenceType.SecurityScheme
				}
			};
			
			c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
			
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{ jwtSecurityScheme, Array.Empty<string>() }
			});
		});


		return services;
	}
}