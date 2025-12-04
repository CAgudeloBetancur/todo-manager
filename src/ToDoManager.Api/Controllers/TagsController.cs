using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoManager.Application.Tags.Commands.CreateTag;
using ToDoManager.Application.Tags.Commands.RemoveTag;
using ToDoManager.Application.Tags.Commands.UpdateTag;
using ToDoManager.Application.Tags.Common;
using ToDoManager.Application.Tags.Queries.GetTagByIdQuery;
using ToDoManager.Application.Tags.Queries.ListTags;
using ToDoManager.Contracts.Tags;

namespace ToDoManager.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class TagsController : ApiController
{
	private readonly ISender _sender;

	public TagsController(ISender sender)
	{
		_sender = sender;
	}

	[HttpPost()]
	public async Task<IActionResult> Create([FromBody]DefaultTagRequest tagRequest)
	{
		var createResult = await _sender.Send( new CreateTagCommand(tagRequest.Name) );

		return createResult.Match(
			result => CreatedAtAction(nameof(GetById), new {id = result.Id}, new DefaultTagResponse(result.Id, result.Name)),
			errors => Problem( errors )
			);
	}

	[HttpGet()]
	public async Task<IActionResult> List()
	{
		var queryResult = await _sender.Send(new ListTagsQuery());
		return Ok(queryResult);
	}
	
	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(Guid id)
	{
		var requestResult = await _sender.Send(new GetTagByIdQuery(id));

		return requestResult.Match(
			result => Ok(new DefaultTagResponse(result.Id, result.Name)),
			errors => Problem(errors)
			);
	}
	
	[HttpDelete("{id}") ]
	public async Task<IActionResult> DeleteById(Guid id)
	{
		var deleteResult = await _sender.Send(new DeleteTagCommand(id));

		return deleteResult.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}
	
	[HttpPut("{id}") ]
	public async Task<IActionResult> Update(Guid id, [FromBody] DefaultTagRequest request)
	{
		var updateResult = await _sender.Send(new UpdateTagCommand(id, request.Name));

		return updateResult.Match(
			_ => NoContent(),
			errors => Problem(errors)
		);
	}
	
}