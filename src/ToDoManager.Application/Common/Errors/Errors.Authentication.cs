using ErrorOr;

namespace ToDoManager.Application.Common.Errors;

public static partial class Errors
{
	public static class Authentication
	{
		public static Error InvalidCredentials =>
			Error.Validation(code: "Auth.InvalidCred", description: "Invalid credentials.");
		
		public static Error UserIdNotFound =>
			Error.Unauthorized("Auth.UserIdNotFound", "User ID claim was not found.");

		public static Error InvalidUserId =>
			Error.Unauthorized("Auth.InvalidUserId", "User ID claim is invalid.");
		
		public static Error ForbiddenAccess =>
			Error.Unauthorized("Auth.ForbiddenAccess", "You do not have permission to view other users' todos.");
	}
}