using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;

namespace ToDoManager.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
	where TRequest : notnull
	where TResponse : IErrorOr
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

	public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
	}
	
	public async Task<TResponse> Handle(
		TRequest request, 
		RequestHandlerDelegate<TResponse> next, 
		CancellationToken cancellationToken
		)
	{
		if (request is IQuery<TResponse>) return await next();

		var strategy = _unitOfWork.CreateExecutionStrategy();

		return await strategy.ExecuteAsync(
			state: (request, next, _unitOfWork, _logger),
			operation: async (dbContext, state, cancelToken) =>
			{
				var (req, handlerDelegate, unitOfWork, logger) = state;
				
				await unitOfWork.BeginTransactionAsync();

				try
				{
					var response = await handlerDelegate();

					if (response.IsError)
					{
						await unitOfWork.RollbackAsync();
						return response;
					} 
						
					await unitOfWork.SaveChangesAsync(cancelToken);
					await unitOfWork.CommitAsync();
					
					return response;
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error in transaction for {Request}", typeof(TRequest).Name);
					await unitOfWork.RollbackAsync();
					throw;
				}
			},
			verifySucceeded: null,
			cancellationToken: cancellationToken
			);
	}	
}