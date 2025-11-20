namespace ToDoManager.Application.Authentication.Common.Persistence;

public class AuthenticationError
{
	public string Code { get; }
	public string Description { get; }

	public AuthenticationError(string code, string description)
	{
		Code = code;
		Description = description;
	}
}