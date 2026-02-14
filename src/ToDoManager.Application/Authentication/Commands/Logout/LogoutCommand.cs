using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Authentication.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<ErrorOr<Unit>>;