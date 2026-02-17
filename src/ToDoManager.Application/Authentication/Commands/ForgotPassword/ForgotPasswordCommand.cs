using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Authentication.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand<ErrorOr<Unit>>;
