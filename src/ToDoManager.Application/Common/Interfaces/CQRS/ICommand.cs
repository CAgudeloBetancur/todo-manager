using MediatR;

namespace ToDoManager.Application.Common.Interfaces.CQRS;

public interface ICommand<TResponse> : IRequest<TResponse> { }