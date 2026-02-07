using MediatR;

namespace ToDoManager.Application.Common.Interfaces.CQRS;

public interface IQuery<TResponse> : IRequest<TResponse> { }