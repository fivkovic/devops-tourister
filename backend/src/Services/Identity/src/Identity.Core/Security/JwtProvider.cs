using Identity.Core.Model;
using Microsoft.IdentityModel.Tokens;
using Shared.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Core.Security;

internal static class JwtProvider
{
    private static readonly JwtSecurityTokenHandler _tokenHandler = new();

    public static string GetAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (ClaimTypes.Role, user.Role),
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(90),
            SigningCredentials = KeyProvider.SigningCredentials,
            Issuer = KeyProvider.Issuer,
            Audience = KeyProvider.Audience
        };

        var token = _tokenHandler.CreateToken(descriptor);

        return _tokenHandler.WriteToken(token);
    }
}