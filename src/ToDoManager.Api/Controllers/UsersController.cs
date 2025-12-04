using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Users.Queries.GetTodosForUser;

namespace ToDoManager.Api.Controllers;

[Authorize] // Admin
[Route("api/[controller]")]
public class UsersController : ApiController
{
	private readonly ISender _sender;

	public UsersController(ISender sender)
	{
		_sender = sender;
	}

	[HttpGet("{userId}/todos")]
	public async Task<IActionResult> TodosForUser(Guid userId)
	{
		var queryResult = await _sender.Send(new GetTodosForUserQuery(userId));
		
		return queryResult.Match(
			result =>  Ok(result),
			errors => Problem(errors)
			);
	}
}