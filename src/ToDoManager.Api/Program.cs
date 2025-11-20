
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;
using ToDoManager.Api.Middleware;

public class TodoManagerApi
{
	private readonly WebApplication _app;
	
	public TodoManagerApi(
		string[] args, 
		Action<IServiceCollection, IConfiguration> options
		)
	{
		var builder = WebApplication.CreateBuilder(args);
		
		builder.Logging.AddSerilog();
		
		options.Invoke(builder.Services, builder.Configuration);

		_app = builder.Build();
		
		_app.UseRouting();
		
		_app.UseStatusCodePages(async context =>
		{
			var response = context.HttpContext.Response;

			if (
				response.StatusCode == StatusCodes.Status401Unauthorized ||
				response.StatusCode == StatusCodes.Status403Forbidden
			)
			{
				response.ContentType = "application/problem+json";

				var problem = new ProblemDetails
				{
					Status = response.StatusCode,
					Title = response.StatusCode == 401 ? "Unauthorized" : "Forbidden",
					Detail = "You are not authorized to access this resource"
				};
				
				await response.WriteAsJsonAsync(problem);
			}
		});

		// _app.UseMiddleware<SerilogEnrichmentMiddleware>();

		// Configure the HTTP request pipeline.
		
		if (_app.Environment.IsDevelopment())
		{
			_app.UseSwagger();
			_app.UseSwaggerUI();
		}
		
		_app.UseHttpsRedirection();
		
		_app.UseExceptionHandler();
		_app.UseAuthentication();
		_app.UseAuthorization();

		_app.MapControllers();
	}

	public Task StartAsync()
	{
		return _app.RunAsync();
	}
    
}

