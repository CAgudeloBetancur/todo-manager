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

	public async Task<Error?> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			await _context.SaveChangesAsync(cancellationToken);
			return null;
		}
		catch (DbUpdateConcurrencyException ex)
		{
			_logger.LogWarning(ex, "Concurrency conflict detected while saving changes.");
			return Errors.Persistence.ConcurrencyConflict;
		}
		catch (DbUpdateException ex) when  (ex.InnerException is NpgsqlException sqlEx)
		{
			_logger.LogWarning(ex, "Database constraint violation: {@SqlState}.", sqlEx.SqlState);
			return sqlEx.SqlState switch
			{
				"23503" => Errors.Persistence.ForeignKeyViolation,
				"23505" => Errors.Persistence.UniqueConstraintViolation,
				"23502" => Errors.Persistence.NullConstraintViolation,
				_ => Errors.Persistence.SaveFailure
			};
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unexpected error while saving changes.");
			return Errors.Persistence.Unexpected;
		}
	}
}