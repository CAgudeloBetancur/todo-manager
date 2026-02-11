using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Authentication.Commands.ChangeEmail;

public record class ChangeEmailCommand(string NewEmail) : IRequest<ErrorOr<Unit>>;