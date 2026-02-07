using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Application.Tags.Common;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Queries.GetTagById;

public record GetTagByIdQuery(Guid TagId) : IQuery<ErrorOr<DefaultTagResult>>;