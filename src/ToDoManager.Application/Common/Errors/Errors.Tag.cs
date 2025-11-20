using ErrorOr;

namespace ToDoManager.Application.Common.Errors;

public static partial class Errors
{
	public static class Tag
	{
		public static Error NotFound => 
			Error.NotFound("Tags.NotFound", "The requested tag was not found.");
	}
}