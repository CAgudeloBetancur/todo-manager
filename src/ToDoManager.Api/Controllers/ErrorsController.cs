using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ToDoManager.Api.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[ApiVersionNeutral]
[ApiController]
public class ErrorsController : ControllerBase
{
	[HttpGet("/error")]
	public IActionResult Error()
	{
		return Problem();
	}
}