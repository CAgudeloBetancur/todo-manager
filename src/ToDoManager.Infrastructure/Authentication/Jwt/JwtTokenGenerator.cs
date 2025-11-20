using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDoManager.Application.Common.Interfaces.Authentication;
using ToDoManager.Application.Common.Interfaces.Services;
using ToDoManager.Domain.Users;

namespace ToDoManager.Infrastructure.Authentication.Jwt;

public class JwtTokenGenerator : IJwtTokenGenerator
{
	private readonly JwtSettings _jwtSettings;
	private readonly IDateTimeProvider _dateTimeProvider;

	public JwtTokenGenerator(IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtOptions)
	{
		_dateTimeProvider = dateTimeProvider;
		_jwtSettings = jwtOptions.Value;
	}

	public string GenerateToken(User user)
	{
		var signingCredentials = new SigningCredentials(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
			SecurityAlgorithms.HmacSha256
			);

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
			new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
			new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var securityToken = new JwtSecurityToken(
			issuer:  _jwtSettings.Issuer,
			audience: _jwtSettings.Audience,
			expires: _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
			claims: claims,
			signingCredentials: signingCredentials
			);
		
		return new JwtSecurityTokenHandler().WriteToken(securityToken);
	}
}