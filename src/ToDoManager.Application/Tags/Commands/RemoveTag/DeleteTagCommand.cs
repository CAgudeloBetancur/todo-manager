using ErrorOr;
using MediatR;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.RemoveTag;

public record DeleteTagCommand(TagId TagId) : IRequest<ErrorOr<Unit>>;