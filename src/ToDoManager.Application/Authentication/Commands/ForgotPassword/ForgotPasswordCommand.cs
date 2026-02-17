using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Authentication.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<ErrorOr<Unit>>;
