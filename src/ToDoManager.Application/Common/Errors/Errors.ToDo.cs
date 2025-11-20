using ErrorOr;

namespace ToDoManager.Application.Common.Errors;

public static partial class Errors
{
	public static class ToDo
	{
		public static Error NotFound => 
			Error.NotFound("Todos.NotFound", "The requested todo was not found.");
	}
}