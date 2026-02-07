using ErrorOr;
using Microsoft.EntityFrameworkCore.Storage;

namespace ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;

public interface IUnitOfWork
{
	IExecutionStrategy CreateExecutionStrategy();
	Task BeginTransactionAsync();
	Task CommitAsync();
	Task RollbackAsync();
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}