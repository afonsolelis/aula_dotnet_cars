using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Volkswagen.Dashboard.Domain.Users;
using Volkswagen.Dashboard.Services.Auth;

namespace Volkswagen.Dashboard.Services.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly byte[] _key;

    public JwtTokenService(string signingKey)
    {
        _key = Encoding.ASCII.GetBytes(signingKey);
    }

    public LoginResponse GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var createdAt = DateTime.UtcNow;
        var expiresAt = createdAt.AddHours(2);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email.Value)
            }),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new LoginResponse
        {
            CreatedAt = createdAt,
            ExpiresAt = expiresAt,
            AccessToken = tokenHandler.WriteToken(token)
        };
    }
}
