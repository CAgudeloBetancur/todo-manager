using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoManager.Application.Common.Behaviors;
using ToDoManager.Application.Tags.Commands.CreateTag;

namespace ToDoManager.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services
			.AddMediatR(cfg =>
			{
				cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
			});
		
		services
			.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		services
			.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		services
			.AddScoped(typeof(IPipelineBehavior<,>), typeof(EnsureUserExistsBehavior<,>));
		services
			.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
		
		services
			.AddValidatorsFromAssemblyContaining<CreateTagCommandValidator>();
		
		return services;
	}
}