
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;
using ToDoManager.Api;
using ToDoManager.Application;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Infrastructure;
using ToDoManager.Infrastructure.Authentication.Identity;
using ToDoManager.Infrastructure.Authentication.Identity.Entities;
using ToDoManager.Infrastructure.Data.Persistence;
using ToDoManager.Infrastructure.Data.Persistence.Repositories;

async Task InitializeApplication(IServiceProvider serviceProvider)
{
	await serviceProvider.SeedRolesAsync();
	await serviceProvider.SeedAdminUserAsync();
}

var api = new TodoManagerApi(
	args, 
	(services, configuration) =>
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
			.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.WriteTo.Console(
				outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
			)
			.WriteTo.OpenTelemetry(x =>
			{
				x.Endpoint = "http://localhost:5341/ingest/otlp/v1/logs";
				x.Protocol = OtlpProtocol.HttpProtobuf;
				x.Headers = new Dictionary<string, string>
				{
					["X-Seq-ApiKey"] = "HxzetwdGjU0uTExgMhgM"
				};
				x.ResourceAttributes = new Dictionary<string, object>
				{
					["service.name"] = "ToDoManager"
				};

			})
			.CreateLogger();
		
		services
			.AddSerilog();

		services
			.AddResiliencePipeline("Default", x =>
			{
				x.AddRetry(new RetryStrategyOptions()
					{
						ShouldHandle = new PredicateBuilder().Handle<Exception>(),
						Delay = TimeSpan.FromSeconds(2),
						MaxRetryAttempts = 2,
						BackoffType = DelayBackoffType.Exponential,
						UseJitter = true
					})
					.AddTimeout(TimeSpan.FromSeconds(30));
			});
		
		services
			.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.ReportApiVersions = true;
				options.ApiVersionReader = new UrlSegmentApiVersionReader();
			})
			.AddMvc()
			.AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV";
				options.SubstituteApiVersionInUrl = true;
			});
		
		services
			.AddPresentation()
			.AddApplication()
			.AddHttpContextAccessor()
			.AddInfrastructure(configuration)
			.AddIdentityCore<ApplicationUser>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 6;
			})
			.AddRoles<IdentityRole<Guid>>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

	},
	InitializeApplication
	);

try
{
	await api.StartAsync();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application startup failed");
}
finally
{
	Log.CloseAndFlush();
}
