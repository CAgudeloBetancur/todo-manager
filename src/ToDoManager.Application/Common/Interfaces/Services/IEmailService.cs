namespace ToDoManager.Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string toEmail, 
        string resetToken, 
        CancellationToken cancellationToken = default
        );
}
