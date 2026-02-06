using ErrorOr;

namespace ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;

public interface IUnitOfWork
{
	Task BeginTransaction();
	Task CommitAsync();
	Task RollbackAsync();
	Task<Error?> SaveChangesAsync(CancellationToken cancellationToken = default);
}