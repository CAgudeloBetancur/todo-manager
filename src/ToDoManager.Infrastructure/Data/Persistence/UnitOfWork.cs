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
		_currentTransaction = await _context.Database.BeginTransactionAsync();
	}

	public async Task CommitAsync()
	{
		await _context.SaveChangesAsync();

		if (_currentTransaction != null)
		{
			await _currentTransaction.CommitAsync();
			await _currentTransaction.DisposeAsync();
		}
	}

	public async Task RollbackAsync()
	{
		if (_currentTransaction != null)
		{
			await _currentTransaction.RollbackAsync();
			await _currentTransaction.DisposeAsync();
		}
	}

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return await _context.SaveChangesAsync(cancellationToken);
	}
}