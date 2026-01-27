using ErrorOr;
using MediatR;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(TagId TagId) : IRequest<ErrorOr<Unit>>;