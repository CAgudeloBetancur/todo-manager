using System.Runtime.InteropServices.JavaScript;
using ErrorOr;
using MediatR;
using ToDoManager.Application.Authentication.Common;
using ToDoManager.Application.Common.Errors;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Domain.Users;
using ToDoManager.Domain.Users.ValueObjects;

namespace ToDoManager.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
	private readonly IUserRepository _userRepository;
	private readonly IJwtTokenGenerator _jwtTokenGenerator;

	public RegisterCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
	{
		_userRepository = userRepository;
		_jwtTokenGenerator = jwtTokenGenerator;
	}

	public async Task< ErrorOr<AuthenticationResult> > Handle(RegisterCommand request, CancellationToken cancellationToken)
	{
		if (await _userRepository.FindByEmailAsync(request.Email) is not null)
		{
			return Errors.User.DuplicatedEmail;
		}

		var user = User.Create(request.Email, request.Email, request.FirstName, request.LastName);
		
		var creationResult = await _userRepository.AddAsync(user, request.Password);
		
		if(!creationResult.Succeeded) 
			return creationResult.Errors
				.Select(e => Error.Validation(e.Code, e.Description))
				.ToList();

		var userRole = new List<string> { "User" };

		var token = _jwtTokenGenerator.GenerateToken(user, userRole);
		
		return new AuthenticationResult(user, token);
	}
}