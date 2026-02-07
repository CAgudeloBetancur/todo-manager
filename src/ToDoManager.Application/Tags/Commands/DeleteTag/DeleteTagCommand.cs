using ErrorOr;
using MediatR;
using ToDoManager.Application.Common.Interfaces.CQRS;
using ToDoManager.Domain.Tags.ValueObjects;

namespace ToDoManager.Application.Tags.Commands.DeleteTag;

public record DeleteTagCommand(Guid TagId) : ICommand<ErrorOr<Unit>>;