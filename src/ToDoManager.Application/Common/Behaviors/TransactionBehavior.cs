using MediatR;
using Microsoft.Extensions.Logging;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;

namespace ToDoManager.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
	where TRequest : notnull
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

		await _unitOfWork.BeginTransaction();

		try
		{
			var response = await next();
			
			await _unitOfWork.SaveChangesAsync(cancellationToken);
			await _unitOfWork.CommitAsync();

			return response;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in transaction for {Request}", typeof(TRequest).Name);
			await _unitOfWork.RollbackAsync();
			throw;
		}
	}	
}