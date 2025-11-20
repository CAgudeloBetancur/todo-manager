using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Todos.Commands.CreateTodo;
using ToDoManager.Application.Todos.Commands.DeleteTodo;
using ToDoManager.Application.Todos.Commands.UpdateTodo;
using ToDoManager.Application.Todos.Queries.GetTodoById;
using ToDoManager.Application.Todos.Queries.GetTodosByUser;
using ToDoManager.Contracts.Todos;

namespace ToDoManager.Api.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class TodosController : ApiController
{
	private readonly ISender _sender;

	public TodosController(ISender sender)
	{
		_sender = sender;
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody]CreateTodoRequest request)
	{
		if (!DateTime.TryParse(request.DueDate, out var dueDate))
		{
			return BadRequest("Invalid due date format");
		}

		var commandResult = await _sender
			.Send(
				new CreateTodoCommand(
					request.Title,
					request.Description,
					dueDate,
					request.Priority,
					request.Status
					)
				);

		return commandResult.Match(
			result => Ok(result),
			errors => Problem(errors)
			);
	}

	[HttpGet]
	public async Task<IActionResult> GetTodoById([FromQuery]GetTodoByIdRequest query)
	{
		var queryResult = await _sender.Send( new  GetTodoByIdQuery(query.TodoId) );

		return queryResult.Match(
			result => Ok(result),
			errors => Problem(errors)
			);
	}
	
	[HttpGet]
	public async Task<IActionResult> GetTodosByUser([FromQuery]GetTodosByUserRequest query)
	{
		var queryResult = await _sender.Send( new  GetTodosByUserQuery(query.UserId) );
		
		return queryResult.Match(
			result => Ok(result),
			errors => Problem(errors)
			);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(
		Guid id, 
		[FromBody] UpdateTodoRequest request
		)
	{
		if (!DateTimeOffset.TryParse(request.DueDate, out var dueDate))
		{
			return BadRequest("Invalid due date format");
		}
		
		var requestResult = await _sender
			.Send(
				new UpdateTodoCommand(
					id,
					request.Title,
					request.Description,
					dueDate.UtcDateTime,
					request.Priority,
					request.Status
					)
				);

		return requestResult.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		var deleteResult = await _sender.Send(new DeleteTodoCommand(id));

		return deleteResult.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}
}