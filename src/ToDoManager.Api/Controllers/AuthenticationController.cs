using System.Text.RegularExpressions;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Authentication.Commands.ChangeEmail;
using ToDoManager.Application.Authentication.Commands.ChangePassword;
using ToDoManager.Application.Authentication.Commands.Refresh;
using ToDoManager.Application.Authentication.Commands.Register;
using ToDoManager.Application.Authentication.Commands.UpdateUser;
using ToDoManager.Application.Authentication.Queries.Login;
using ToDoManager.Contracts.Authentication;
using LoginRequest = ToDoManager.Contracts.Authentication.LoginRequest;
using RegisterRequest = ToDoManager.Contracts.Authentication.RegisterRequest;

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
				_ => NoContent(),
				errors => Problem(errors)
				);
	}

	[HttpPost]
	[Authorize]
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

	[HttpPut]
	[Authorize]
	public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
	{
		var authResult = await _sender
			.Send(new UpdateUserCommand(request.Email, request.FirstName, request.LastName));

		return authResult
			.Match(
				_ => NoContent(),
				errors => Problem(errors)
				);
	}

	[HttpPatch]
	[Authorize]
	public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
	{
		var authResult = await _sender.Send(new ChangeEmailCommand(request.Email));

		return authResult
			.Match(
				_ => NoContent(),
				errors => Problem(errors)
				);
	}
	
	[HttpPatch]
	[Authorize]
	public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
	{
		var authResult = await _sender
			.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword));

		return authResult
			.Match(
				_ => NoContent(),
				errors => Problem(errors)
			);
	}

	[HttpPost]
	public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
	{
		var refreshResult = await _sender.Send(new RefreshCommand(request.RefreshToken));

		return refreshResult.Match(
			result => Ok(result),
			errors => Problem(errors));
	}
}