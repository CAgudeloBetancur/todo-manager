using Microsoft.Extensions.Logging;
using ToDoManager.Application.Common.Interfaces.Services;

namespace ToDoManager.Infrastructure.Services;

public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetEmailAsync(
        string toEmail, 
        string resetToken, 
        CancellationToken cancellationToken = default
        )
    {
        _logger.LogInformation(
            "=== PASSWORD RESET EMAIL ===\n" +
            "To: {Email}\n" +
            "Reset Token: {Token}\n" +
            "============================",
            toEmail,
            resetToken);

        return Task.CompletedTask;
    }
}
