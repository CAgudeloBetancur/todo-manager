using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Persistence;
using ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;

namespace ToDoManager.Infrastructure.Data.Persistence;

public class UnitOfWork : IUnitOfWork
{
	private readonly ApplicationDbContext _context;
	private IDbContextTransaction? _currentTransaction;
	private readonly ILogger<UnitOfWork> _logger;
	
	public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task BeginTransaction()
	{
		_logger.LogInformation("Beginning transaction");
		_currentTransaction = await _context.Database.BeginTransactionAsync();
	}

	public async Task CommitAsync()
	{
		await _context.SaveChangesAsync();

		if (_currentTransaction != null)
		{
			_logger.LogInformation("Commiting transaction");
			await _currentTransaction.CommitAsync();
			
			_logger.LogInformation("Disposing transaction");
			await _currentTransaction.DisposeAsync();
		}
	}

	public async Task RollbackAsync()
	{
		if (_currentTransaction != null)
		{
			_logger.LogInformation("Rolling back transaction");
			await _currentTransaction.RollbackAsync();
			
			_logger.LogInformation("Disposing transaction");
			await _currentTransaction.DisposeAsync();
		}
	}

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Saving changes");
		return await _context.SaveChangesAsync(cancellationToken);
	}
}