using MediatR;
using ToDoManager.Application.Common.Interfaces.Http;

namespace ToDoManager.Application.Common.Behaviors;

public class EnsureUserExistsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
	where TRequest : notnull
{
	private readonly IUserAccessor _userAccessor;
	
	public EnsureUserExistsBehavior(IUserAccessor userAccessor)
	{
		_userAccessor = userAccessor;
	}
	
	public async Task<TResponse> Handle(
		TRequest request, 
		RequestHandlerDelegate<TResponse> next, 
		CancellationToken cancellationToken
		)
	{
		
		if(!_userAccessor.IsUserLoggedIn()) 
			throw new UnauthorizedAccessException("User not authenticated.");

		return await next();
	}
}