using Asp.Versioning;
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
[ApiVersion("1.0", Deprecated = false)]
[ApiVersion("2.0", Deprecated = false)]
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public class TagsController : ApiController
{
	private readonly ISender _sender;

	public TagsController(ISender sender)
	{
		_sender = sender;
	}

	[HttpPost()]
	[MapToApiVersion("1.0")]
	public async Task<IActionResult> Create([FromBody]DefaultTagRequest tagRequest)
	{
		var createResult = await _sender.Send( new CreateTagCommand(tagRequest.Name) );

		return createResult.Match(
			result => CreatedAtAction(nameof(GetById), new {id = result.Id}, new DefaultTagResponse(result.Id, result.Name)),
			errors => Problem( errors )
			);
	}
	
	[HttpPost()]
	[MapToApiVersion("2.0")]
	public async Task<IActionResult> CreateV2([FromBody]DefaultTagRequest tagRequest)
	{
		return Ok("create v2");
	}

	[HttpGet()]
	[MapToApiVersion("1.0")]
	public async Task<IActionResult> List()
	{
		var queryResult = await _sender.Send(new ListTagsQuery());
		return Ok(queryResult);
	}
	
	[HttpGet("{id}")]
	[MapToApiVersion("1.0")]
	public async Task<IActionResult> GetById(Guid id)
	{
		var requestResult = await _sender.Send(new GetTagByIdQuery(id));

		return requestResult.Match(
			result => Ok(new DefaultTagResponse(result.Id, result.Name)),
			errors => Problem(errors)
			);
	}
	
	[HttpDelete("{id}") ]
	[MapToApiVersion("1.0")]
	public async Task<IActionResult> DeleteById(Guid id)
	{
		var deleteResult = await _sender.Send(new DeleteTagCommand(id));

		return deleteResult.Match(
			_ => NoContent(),
			errors => Problem(errors)
			);
	}
	
	[HttpPut("{id}") ]
	[MapToApiVersion("1.0")]
	public async Task<IActionResult> Update(Guid id, [FromBody] DefaultTagRequest request)
	{
		var updateResult = await _sender.Send(new UpdateTagCommand(id, request.Name));

		return updateResult.Match(
			_ => NoContent(),
			errors => Problem(errors)
		);
	}
	
}