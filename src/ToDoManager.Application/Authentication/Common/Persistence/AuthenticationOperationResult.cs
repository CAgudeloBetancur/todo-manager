using ErrorOr;

namespace ToDoManager.Application.Authentication.Common.Persistence;

public class AuthenticationOperationResult
{
	public bool Succeeded { get; }
	public List<AuthenticationError> Errors { get; }
	
	private AuthenticationOperationResult(bool succeeded, List<AuthenticationError> errors)
	{
		Succeeded = succeeded;
		Errors = errors;
	}

	public static AuthenticationOperationResult Success() 
		=> new(true, new List<AuthenticationError>());
	
	public static AuthenticationOperationResult Failure(IEnumerable<AuthenticationError> errors) 
		=> new(false, errors.ToList());
}
