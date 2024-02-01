using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Shared.Security;

namespace Shared.Swagger;

public static class Extensions
{
    public static void AddSwagger(this IServiceCollection services, string serviceName)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
            });

            options.OperationFilter<AuthFilter>();

            options.CustomSchemaIds(type => type.ToString().Replace("+", "."));
            options.SwaggerDoc("v1", new OpenApiInfo { Title = serviceName, Version = "v1" });
        });
    }
}