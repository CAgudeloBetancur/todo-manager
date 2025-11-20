using ErrorOr;
using MediatR;

namespace ToDoManager.Application.Tags.Commands.UpdateTag;

public record class UpdateTagCommand(Guid Id, string Name) : IRequest<ErrorOr<Unit>>;