using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Shared.Swagger;

public static class Extensions
{
    public static void AddSwagger(this IServiceCollection services, string serviceName)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => type.ToString().Replace("+", "."));
            options.SwaggerDoc("v1", new OpenApiInfo { Title = serviceName, Version = "v1" });
        });
    }
}