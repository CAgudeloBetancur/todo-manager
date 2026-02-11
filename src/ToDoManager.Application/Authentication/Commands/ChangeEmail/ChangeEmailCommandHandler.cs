using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Authentication.Commands.ChangeEmail;

public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, ErrorOr<Unit>>
{
	public Task<ErrorOr<Unit>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}