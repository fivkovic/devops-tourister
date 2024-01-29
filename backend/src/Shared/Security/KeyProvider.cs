using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Shared.Security;

public static class KeyProvider
{
    private const string _developmentJwtSecret = "/wMLPRs1zvFxIsIztaakWJq7TWF+Lg==";

    public const string Issuer = "https://api.tourister.com";
    public const string Audience = "https://tourister.com";

    public static readonly SigningCredentials SigningCredentials;

    static KeyProvider()
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _developmentJwtSecret;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
    }
}