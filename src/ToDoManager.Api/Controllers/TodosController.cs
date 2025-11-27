using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Todos.Commands.CreateTodo;
using ToDoManager.Application.Todos.Commands.DeleteTodo;
using ToDoManager.Application.Todos.Commands.UpdateTodo;
using ToDoManager.Application.Todos.Queries.GetTodoById;
using ToDoManager.Application.Todos.Queries.GetTodosByUser;
using ToDoManager.Application.Todos.SubTodos.Commands.CreateSubTodo;
using ToDoManager.Application.Todos.SubTodos.Commands.RemoveSubTodo;
using ToDoManager.Application.Todos.SubTodos.Commands.ReorderSubTodos;
using ToDoManager.Application.Todos.SubTodos.Commands.UpdateSubTodo;
using ToDoManager.Contracts.Todos;
using ToDoManager.Contracts.Todos.SubTodos;

namespace ToDoManager.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class TodosController : ApiController
{
	private readonly ISender _sender;

	public TodosController(ISender sender)
	{
		_sender = sender;
	}

	[HttpPost("create")]
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

	[HttpGet("get-by-id")]
	public async Task<IActionResult> GetTodoById([FromQuery]GetTodoByIdRequest query)
	{
		var queryResult = await _sender.Send( new  GetTodoByIdQuery(query.TodoId) );

		return queryResult.Match(
			result => Ok(result),
			errors => Problem(errors)
			);
	}
	
	[HttpGet("get-by-user")]
	public async Task<IActionResult> GetTodosByUser([FromQuery]GetTodosByUserRequest query)
	{
		var queryResult = await _sender.Send( new  GetTodosByUserQuery(query.UserId) );
		
		return queryResult.Match(
			result => Ok(result),
			errors => Problem(errors)
			);
	}

	[HttpPut("update/{id}")]
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

	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		var deleteResult = await _sender.Send(new DeleteTodoCommand(id));

		return deleteResult.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpPost("{todoId}/subtodos/create")]
	public async Task<IActionResult> CreateSubTodo(Guid todoId, [FromBody] CreateSubTodoRequest request)
	{
		var result = await _sender.Send(new CreateSubTodoCommand(todoId, request.Title, request.Description, request.IsComplete));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpPut("{todoId}/subtodos/re-order")]
	public async Task<IActionResult> ReorderSubTodos(Guid todoId, [FromBody] List<ReorderSubTodosDto> request)
	{
		var result = await _sender.Send(new ReorderSubTodosCommand(todoId, request));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpPut("{todoId}/subtodos/update/{subTodoId}")]
	public async Task<IActionResult> UpdateSubTodo(
		Guid todoId, 
		Guid subTodoId, 
		[FromBody] UpdateSubTodoRequest request
		)
	{
		var result = await _sender.Send(
			new UpdateSubTodoCommand(todoId, subTodoId, request.Title, request.Description, request.IsComplete)
			);

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpDelete("{todoId}/subtodos/delete/{subTodoId}")]
	public async Task<IActionResult> DeleteSubTodo(Guid todoId, Guid subTodoId)
	{
		var result = await _sender.Send(new RemoveSubTodoCommand(todoId, subTodoId));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}
}