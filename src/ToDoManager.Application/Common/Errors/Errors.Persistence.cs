using ErrorOr;

namespace ToDoManager.Application.Common.Errors;

public static partial class Errors
{
	public static class Persistence
	{
		public static Error SaveFailure => 
			Error.Unexpected("Persistence.SaveFailure", "Could not persist changes to the database");
		
		public static Error ConcurrencyConflict =>
			Error.Conflict("Persistence.Concurrency", "A concurrency conflict occurred while saving.");

		public static Error ConnectionFailure =>
			Error.Unexpected("Persistence.Connection", "Database connection failed.");
		
		public static Error ForeignKeyViolation =>
			Error.Conflict("Persistence.ForeignKey", "Operation violates a foreign key constraint.");

		public static Error UniqueConstraintViolation =>
			Error.Conflict("Persistence.UniqueConstraint", "A unique constraint was violated.");

		public static Error Unexpected =>
			Error.Failure("Persistence.Unexpected", "An unexpected error occurred during persistence.");

		public static Error NullConstraintViolation =>
			Error.Validation("Persistence.NullConstraint", "A required field was left empty.");
	}
}