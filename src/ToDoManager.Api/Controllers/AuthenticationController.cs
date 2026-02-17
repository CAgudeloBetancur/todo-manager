using System.Text.RegularExpressions;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Authentication.Commands.ChangeEmail;
using ToDoManager.Application.Authentication.Commands.ChangePassword;
using ToDoManager.Application.Authentication.Commands.ForgotPassword;
using ToDoManager.Application.Authentication.Commands.Logout;
using ToDoManager.Application.Authentication.Commands.Refresh;
using ToDoManager.Application.Authentication.Commands.Register;
using ToDoManager.Application.Authentication.Commands.ResetPassword;
using ToDoManager.Application.Authentication.Commands.UpdateUser;
using ToDoManager.Application.Authentication.Queries.Login;
using ToDoManager.Contracts.Authentication;
using ForgotPasswordRequest = ToDoManager.Contracts.Authentication.ForgotPasswordRequest;
using LoginRequest = ToDoManager.Contracts.Authentication.LoginRequest;
using RegisterRequest = ToDoManager.Contracts.Authentication.RegisterRequest;
using ResetPasswordRequest = ToDoManager.Contracts.Authentication.ResetPasswordRequest;

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
	[AllowAnonymous]
	public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
	{
		var refreshResult = await _sender.Send(new RefreshCommand(request.RefreshToken));

		return refreshResult.Match(
			result => Ok(result),
			errors => Problem(errors));
	}

	[HttpPost]
	public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
	{
		var logoutResult = await _sender.Send(new LogoutCommand(request.RefreshToken));

		return logoutResult
			.Match(
				_ => Ok(new {message = "Session closed with success."}),
				errors => Problem(errors)
				);
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
	{
		var result = await _sender.Send(new ForgotPasswordCommand(request.Email));

		return result.Match(
			_ => Ok(new { message = "If the email exists, a reset link has been sent." }),
			errors => Problem(errors));
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
	{
		var result = await _sender
			.Send(new ResetPasswordCommand(request.Email, request.ResetToken, request.NewPassword)
			);

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors));
	}
}