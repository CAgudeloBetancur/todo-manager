using ErrorOr;

namespace ToDoManager.Application.Common.Interfaces.Persistence.UnitOfWork;

public interface IUnitOfWork
{
	Task<Error?> SaveChangesAsync(CancellationToken cancellationToken = default);
}