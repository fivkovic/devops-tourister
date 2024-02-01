using FluentValidation;
using MassTransit;
using MassTransit.Logging;
using Mediator;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Property.API;
using Property.Core.Consumers;
using Property.Core.Database;
using Property.Core.Services;
using Shared.Security;
using Shared.Swagger;
using System.Text.Json.Serialization;

[assembly: MediatorOptions(Namespace = "Property.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();
builder.Services.AddMediator();

builder.Services.AddDbContextPool<PropertyContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("PG_CONNECTION") ??
                           builder.Configuration.GetConnectionString("Postgres");

    options.UseNpgsql(connectionString);
});

builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(includeNamespace: true));

    config.AddConsumer<CreateUser>();
    config.AddConsumer<UpdateUser>();
    config.AddConsumer<DeleteUser>();
    config.AddConsumer<CancelReservation>();
    config.AddConsumer<AcceptReservation>();
    config.AddConsumer<ReviewProperty>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") ??
                               builder.Configuration.GetConnectionString("RabbitMQ");

        cfg.Host(new Uri(connectionString!));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton(provider =>
{
    var storagePath = Path.Combine(builder.Environment.WebRootPath, "images");
    return new ImageService(storagePath);
});

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger("Property.API");

// Add FluentValidation to Swagger
builder.Services.AddValidatorsFromAssemblyContaining<PropertyContext>();
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Property.API"))
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
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PropertyContext>();
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any()) await context.Database.MigrateAsync();
}

app.MapPrometheusScrapingEndpoint();

Endpoints.Map(app);

app.Run();

public partial class Program { }