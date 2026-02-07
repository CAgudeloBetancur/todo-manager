using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Application.Tags.Common;

namespace ToDoManager.Application.Tags.Queries.ListTags;

public record ListTagsQuery : IQuery<List<DefaultTagResult>>;