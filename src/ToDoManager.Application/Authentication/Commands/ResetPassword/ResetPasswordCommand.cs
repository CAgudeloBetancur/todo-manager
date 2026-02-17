using ErrorOr;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Authentication.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword
    ) : ICommand<ErrorOr<AuthenticationResult>>;
