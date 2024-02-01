using FluentValidation;
using MassTransit;
using MassTransit.Logging;
using Mediator;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Reservation.API;
using Reservation.Core.Consumers;
using Reservation.Core.Database;
using Reservation.Core.Services;
using Shared.Security;
using Shared.Swagger;
using System.Text.Json.Serialization;

[assembly: MediatorOptions(Namespace = "Reservation.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddMediator();

builder.Services.AddSingleton<ReservationContext>(o =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION") ??
                           builder.Configuration.GetConnectionString("MongoDB");

    var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
    var options = new InstrumentationOptions { CaptureCommandText = true };
    clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber(options));

    var client = new MongoClient(clientSettings);
    return new ReservationContext(client, databaseName: "reservation");
});

builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(includeNamespace: true));

    config.AddConsumer<CreateReservation>();
    config.AddConsumer<AcceptReservation>();
    config.AddConsumer<CreateUser>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") ??
                               builder.Configuration.GetConnectionString("RabbitMQ");

        cfg.Host(new Uri(connectionString!));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<NotificationService>();

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger("Reservation.API");

// Add FluentValidation to Swagger
builder.Services.AddValidatorsFromAssemblyContaining<ReservationContext>();
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Reservation.API"))
    .WithTracing(config => config
        .SetSampler(new AlwaysOnSampler())
        .AddSource(DiagnosticHeaders.DefaultListenerName)
        .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
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

app.MapPrometheusScrapingEndpoint();

app.MapEndpoints();

app.Run();
