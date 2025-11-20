using System.Net.Sockets;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ToDoManager.Api.Common.Errors;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly IProblemDetailsService _problemDetailsService;

	public GlobalExceptionHandler(
		IProblemDetailsService problemDetailsService
		)
	{
		_problemDetailsService = problemDetailsService;
	}
	
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext, 
		Exception exception, 
		CancellationToken cancellationToken
		)
	{
		// log
		
		var statusCode = exception switch
		{
			NpgsqlException or SocketException=> StatusCodes.Status503ServiceUnavailable,
			ApplicationException => StatusCodes.Status400BadRequest,
			_ => StatusCodes.Status500InternalServerError
		};

		httpContext.Response.StatusCode = statusCode; 
		
		return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = new ProblemDetails
			{
				Type = exception.GetType().Name,
				Title = "An error occured",
				Detail = exception.Message
			}
		});
	}

	private static Exception GetInnermostException(Exception ex)
	{
		while(ex.InnerException != null) ex = ex.InnerException;
		return ex;
	}
}