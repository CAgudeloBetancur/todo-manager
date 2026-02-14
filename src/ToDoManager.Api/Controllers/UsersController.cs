using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Users.Queries.GetTodosForUser;

namespace ToDoManager.Api.Controllers;

[Authorize]
[ApiVersion("1.0", Deprecated = false)]
// [ApiVersion("2.0")]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class UsersController : ApiController
{
	private readonly ISender _sender;

	public UsersController(ISender sender)
	{
		_sender = sender;
	}

	[HttpGet("{userId}/todos")]
	[Authorize(Policy = "OnlyAdmin")]
	public async Task<IActionResult> TodosForUser(Guid userId)
	{
		var queryResult = await _sender.Send(new GetTodosForUserQuery(userId));
		
		return queryResult.Match(
			result =>  Ok(result),
			errors => Problem(errors)
			);
	}
}