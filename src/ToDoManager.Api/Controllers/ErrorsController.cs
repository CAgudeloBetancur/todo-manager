using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ToDoManager.Api.Controllers;

[ApiController]
public class ErrorsController : ControllerBase
{
	[HttpGet("/error")]
	public IActionResult Error()
	{
		return Problem();
	}
}