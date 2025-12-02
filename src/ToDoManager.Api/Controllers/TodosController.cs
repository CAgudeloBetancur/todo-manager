using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Todos.Commands.AddTagIdToTodo;
using ToDoManager.Application.Todos.Commands.ClearTagIdsFromTodo;
using ToDoManager.Application.Todos.Commands.CreateTodo;
using ToDoManager.Application.Todos.Commands.RemoveTagIdFromTodo;
using ToDoManager.Application.Todos.Commands.RemoveTodo;
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

	[HttpGet("{todoId}")]
	public async Task<IActionResult> GetTodoById(Guid todoId)
	{
		var queryResult = await _sender.Send( new  GetTodoByIdQuery(todoId) );

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

	[HttpPut("{todoId}")]
	public async Task<IActionResult> Update(
		Guid todoId, 
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
					todoId,
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

	[HttpDelete("{todoId}")]
	public async Task<IActionResult> Delete(Guid todoId)
	{
		var deleteResult = await _sender.Send(new DeleteTodoCommand(todoId));

		return deleteResult.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpPost("{todoId}/subtodos")]
	public async Task<IActionResult> CreateSubTodo(Guid todoId, [FromBody] CreateSubTodoRequest request)
	{
		var result = await _sender.Send(new CreateSubTodoCommand(todoId, request.Title, request.Description, request.IsComplete));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpPut("{todoId}/subtodos")]
	public async Task<IActionResult> ReorderSubTodos(Guid todoId, [FromBody] List<ReorderSubTodosDto> request)
	{
		var result = await _sender.Send(new ReorderSubTodosCommand(todoId, request));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpPut("{todoId}/subtodos/{subTodoId}")]
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

	[HttpDelete("{todoId}/subtodos/{subTodoId}")]
	public async Task<IActionResult> DeleteSubTodo(Guid todoId, Guid subTodoId)
	{
		var result = await _sender.Send(new RemoveSubTodoCommand(todoId, subTodoId));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}

	[HttpPost("{todoId}/tags")]
	public async Task<IActionResult> AddTagIdToTodo(
		Guid todoId,
		[FromBody] AddTagIdToTodoRequest request
		)
	{
		var result = await _sender.Send(new AddTagIdToTodoCommand(todoId, request.TagId));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}
	
	[HttpDelete("{todoId}/tags/{tagId}")]
	public async Task<IActionResult> RemoveTagIdFromTodo(Guid todoId, Guid tagId)
	{
		var result = await _sender.Send(new RemoveTagIdFromTodoCommand(todoId, tagId));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
		);
	}
	
	[HttpDelete("{todoId}/tags")]
	public async Task<IActionResult> RemoveTagIdFromTodo(Guid todoId)
	{
		var result = await _sender.Send(new ClearTagIdsFromTodoCommand(todoId));

		return result.Match(
			_ => NoContent(),
			errors => Problem(errors)
		);
	}
}