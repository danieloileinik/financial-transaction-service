using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application;
using Application.Abstractions.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

public class JwtTokenService(IOptions<JwtSettings> jwtSettings) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string GenerateUserToken(Guid accountId, string role = "User")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, accountId.ToString()),
            new(ClaimTypes.Role, role)
        };
        return GenerateToken(claims);
    }

    public string GenerateAdminToken()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Admin")
        };
        return GenerateToken(claims);
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}