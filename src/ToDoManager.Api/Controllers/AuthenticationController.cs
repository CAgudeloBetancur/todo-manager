using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Authentication.Commands.Register;
using ToDoManager.Application.Authentication.Queries.Login;
using ToDoManager.Contracts.Authentication;

namespace ToDoManager.Api.Controllers;

[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{apiVersion:apiVersion}/[controller]/[action]")]
public class AuthenticationController : ApiController
{
	private readonly ISender _sender;

	public AuthenticationController(ISender sender)
	{
		_sender = sender;
	}

	[HttpPost]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request)
	{
		var authResult = await _sender
			.Send(
				new RegisterCommand(request.FirstName, request.LastName, request.Email, request.Password)
			);

		return authResult
			.Match(
				result => Ok(result),
				errors => Problem(errors)
				);
	}

	[HttpPost]
	public async Task<IActionResult> Login([FromBody] LoginRequest request)
	{
		var authResult = await _sender
			.Send(
				new LoginQuery(request.Email, request.Password)
			);

		return authResult
			.Match(
				result => Ok(result),
				errors => Problem(errors)
				);
	}
}