using ErrorOr;
using MediatR;
using ToDoManager.Application.Tags.Common;

namespace ToDoManager.Application.Tags.Commands.CreateTag;

public record CreateTagCommand(string Name) : IRequest< ErrorOr<DefaultTagResult> >;