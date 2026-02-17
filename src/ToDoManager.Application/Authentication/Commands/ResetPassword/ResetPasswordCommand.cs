using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Interfaces;
using ToDoManager.Application.Common.Interfaces.CQRS;
using Unit = MediatR.Unit;

namespace ToDoManager.Application.Authentication.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword
    ) : ICommand<ErrorOr<Unit>>, IAllowAnonymous;
