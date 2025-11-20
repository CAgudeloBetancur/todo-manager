using ErrorOr;
using MediatR;
using ToDoManager.Application.Tags.Common;

namespace ToDoManager.Application.Tags.Queries.ListTags;

public record ListTagsQuery : IRequest<List<DefaultTagResult>>;