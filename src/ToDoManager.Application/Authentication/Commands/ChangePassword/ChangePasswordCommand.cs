using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Authentication.Commands.ChangePassword;

public record class ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<ErrorOr<Unit>>;