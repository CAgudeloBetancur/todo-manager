namespace ToDoManager.Contracts.Authentication;

public record class UpdateUserRequest(
	string Email,
	string FirstName, 
	string LastName
	);