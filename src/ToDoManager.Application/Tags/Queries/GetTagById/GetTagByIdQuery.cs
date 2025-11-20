using ErrorOr;
using MediatR;
using ToDoManager.Application.Tags.Common;

namespace ToDoManager.Application.Tags.Queries.GetTagByIdQuery;

public record GetTagByIdQuery(Guid Id) : IRequest<ErrorOr<DefaultTagResult>>;