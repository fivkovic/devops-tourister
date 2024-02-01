using FluentValidation;
using Identity.API;
using Identity.Core.Consumers;
using Identity.Core.Database;
using Identity.Core.Model;
using MassTransit;
using MassTransit.Logging;
using Mediator;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Security;
using Shared.Swagger;

[assembly: MediatorOptions(Namespace = "Identity.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddMediator();

builder.Services.AddDbContextPool<IdentityContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("PG_CONNECTION") ??
        builder.Configuration.GetConnectionString("Postgres");

    options.UseNpgsql(connectionString);
});

builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(includeNamespace: true));

    config.AddConsumer<UpdateUser>();
    config.AddConsumer<DeleteUser>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") ??
                               builder.Configuration.GetConnectionString("RabbitMQ");

        cfg.Host(new Uri(connectionString!));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<IdentityContext>()
    .AddSignInManager<SignInManager<User>>()
    .AddDefaultTokenProviders();

// Add Authorization
builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

// Add Swagger
builder.Services.AddSwagger("Identity.API");

// Add FluentValidation to Swagger
builder.Services.AddValidatorsFromAssemblyContaining<IdentityContext>();
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Identity.API"))
    .WithTracing(config => config
        .SetSampler(new AlwaysOnSampler())
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(o =>
        {
            o.EnrichWithIDbCommand = (activity, command) =>
            {
                activity.SetTag("db.statement", command.CommandText);
            };
        })
        .AddOtlpExporter(c =>
        {
            var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ??
                           builder.Configuration.GetConnectionString("OTLPExporter");

            c.Endpoint = new Uri(endpoint!);
        }))
    .WithMetrics(config =>
    {
        config.AddPrometheusExporter()
              .AddMeter("Microsoft.AspNetCore.Hosting")
              .AddMeter("Microsoft.AspNetCore.Routing")
              .AddMeter("Microsoft.AspNetCore.Diagnostics")
              .AddMeter("Microsoft.AspNetCore.RateLimiting")
              .AddMeter("Microsoft.AspNetCore.HeaderParsing")
              .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
              .AddMeter("Microsoft.AspNetCore.Http.Connections")
              .AddView(
                    instrumentName: "http.server.request.duration",
                    metricStreamConfiguration: new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10]
                    });
    });

var app = builder.Build();

app.UseCORS();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
        await context.Database.MigrateAsync();
}

app.MapPrometheusScrapingEndpoint();

Endpoints.Map(app);

app.Run();
