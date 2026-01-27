using ErrorOr;
using MediatR;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.UpdateTag;

public record class UpdateTagCommand(TagId TagId, string Name) : IRequest<ErrorOr<Unit>>;