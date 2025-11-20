using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ToDoManager.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> 
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
	where TResponse : IErrorOr
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
	
	public LoggingBehavior(
		ILogger<LoggingBehavior<TRequest, TResponse>> logger
		)
	{
		_logger = logger;
	}


	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		_logger
			.LogInformation(
				"Starting request @{RequestName}, {@DateTimeUtc}",
				request.GetType().Name,
				DateTime.UtcNow
			);

		var result = await next();

		if (result.IsError)
		{
			_logger
				.LogError(
					"Request failure @{RequestName}, {@ErrorInstance},{@DateTimeUtc}",
					request.GetType().Name,
					result.Errors,
					DateTime.UtcNow
				);
		}
		
		_logger
			.LogInformation(
				"Completed request @{RequestName}, {@DateTimeUtc}",
				request.GetType().Name,
				DateTime.UtcNow
			);

		return result;
	}
}