using Microsoft.IdentityModel.Logging;
using Serilog.Context;

namespace ToDoManager.Api.Middleware;

public class SerilogEnrichmentMiddleware
{
	private readonly RequestDelegate _next;

	public SerilogEnrichmentMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var userId = context.User?.Identity?.IsAuthenticated == true 
			? context.User.FindFirst("sub")?.Value ?? context.User.Identity.Name 
			: "anonymous";
		
		var requestId = context.TraceIdentifier;
		
		var requestPath = context.Request.Path;

		using (LogContext.PushProperty("Path", requestPath))
		using (LogContext.PushProperty("UserId", userId))
		using (LogContext.PushProperty("RequestId", requestId))
		{
			await _next(context);
		}
	}
}