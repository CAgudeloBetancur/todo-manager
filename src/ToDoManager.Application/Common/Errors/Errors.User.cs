using ErrorOr;

namespace ToDoManager.Application.Common.Errors;

public static partial class Errors
{
	public static class User
	{
		public static Error DuplicatedEmail => 
			Error.Conflict(code: "User.Duplicated", description: "Email already in use.");
		
		public static Error NotFound => 
			Error.NotFound(code: "User.NotFound", description: "User not found.");
	}
}