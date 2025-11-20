using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Api.Common.Http;

namespace ToDoManager.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
	// GET
	protected IActionResult Problem(List<Error> errors)
	{
		HttpContext.Items[HttpContextItemKeys.Errors] = errors;
		
		var firstError = errors.First();
		
		var statusCode = firstError.Type switch
		{
			ErrorType.Conflict => StatusCodes.Status409Conflict,
			ErrorType.Validation => StatusCodes.Status400BadRequest,
			ErrorType.NotFound => StatusCodes.Status404NotFound,
			_ => StatusCodes.Status500InternalServerError,
		};
		
		return Problem(statusCode: statusCode, title: firstError.Description);
	}
}