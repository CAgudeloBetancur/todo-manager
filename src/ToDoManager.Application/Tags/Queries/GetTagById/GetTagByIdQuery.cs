using ErrorOr;
using MediatR;
using ToDoManager.Application.Tags.Common;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Queries.GetTagById;

public record GetTagByIdQuery(TagId TagId) : IRequest<ErrorOr<DefaultTagResult>>;