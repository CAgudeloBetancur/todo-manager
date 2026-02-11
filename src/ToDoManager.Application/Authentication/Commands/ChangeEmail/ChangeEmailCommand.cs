using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Authentication.Commands.ChangeEmail;

public record class ChangeEmailCommand(Guid UserId, string NewEmail) : IRequest<ErrorOr<Unit>>;