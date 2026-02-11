namespace ToDoManager.Contracts.Authentication;

public record class ChangePasswordRequest(string CurrentPassword, string NewPassword);