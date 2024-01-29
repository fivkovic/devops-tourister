using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

using static Microsoft.Net.Http.Headers.HeaderNames;

namespace Shared.Security;

public static class SecurityExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = KeyProvider.Issuer,
                ValidAudience = KeyProvider.Audience,
                IssuerSigningKey = KeyProvider.SigningCredentials.Key
            };
        });
    }

    public static void UseCORS(this IApplicationBuilder app)
    {
        app.UseCors(c => c.AllowAnyMethod()
                          .WithHeaders(ContentType, Authorization)
                          .AllowCredentials()
                          .SetIsOriginAllowed(origin => true)
                          .SetPreflightMaxAge(TimeSpan.FromHours(2)));
    }

    public static bool IsAuthenticated(this ClaimsPrincipal claims)
    {
        return claims.Identity?.IsAuthenticated ?? false;
    }

    public static Guid Id(this ClaimsPrincipal claims)
    {
        return Guid.TryParse(claims.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
    }

    public static string Role(this ClaimsPrincipal claims)
    {
        return claims.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
}