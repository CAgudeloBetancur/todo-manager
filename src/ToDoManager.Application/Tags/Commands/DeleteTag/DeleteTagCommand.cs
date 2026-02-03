using ErrorOr;
using MediatR;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(Guid TagId) : IRequest<ErrorOr<Unit>>;