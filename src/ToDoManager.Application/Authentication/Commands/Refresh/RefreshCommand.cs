using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;

namespace ToDoManager.Application.Authentication.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<ErrorOr<AuthenticationResult>>;