namespace ToDoManager.Contracts.Authentication;

public record ResetPasswordRequest(string Email, string ResetToken, string NewPassword);
