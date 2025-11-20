
using ToDoManager.Domain.Users;

namespace ToDoManager.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
	string GenerateToken(User user);
}