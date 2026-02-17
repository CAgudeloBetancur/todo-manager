using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;

namespace ToDoManager.Application.Authentication.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword
    ) : IRequest<ErrorOr<AuthenticationResult>>;
