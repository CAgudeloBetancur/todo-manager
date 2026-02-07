using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.UpdateTag;

public record class UpdateTagCommand(Guid TagId, string Name) : ICommand<ErrorOr<Unit>>;