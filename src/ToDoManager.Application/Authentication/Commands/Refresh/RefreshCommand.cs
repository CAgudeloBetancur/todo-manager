using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Authentication.Common.Interfaces;
using ToDoManager.Application.Common.Interfaces.CQRS;

namespace ToDoManager.Application.Authentication.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : ICommand<ErrorOr<AuthenticationResult>>, IAllowAnonymous;