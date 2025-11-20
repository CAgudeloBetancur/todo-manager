using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Tags.Commands.RemoveTag;

public record DeleteTagCommand(Guid Id) : IRequest<ErrorOr<Unit>>;