using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common.Interfaces;

namespace ToDoManager.Application.Authentication.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<ErrorOr<Unit>>;