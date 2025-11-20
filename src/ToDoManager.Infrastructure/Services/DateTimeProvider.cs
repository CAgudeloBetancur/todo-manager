using ToDoManager.Application.Common.Interfaces.Services;

namespace ToDoManager.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
	public DateTime UtcNow => DateTime.UtcNow;
}