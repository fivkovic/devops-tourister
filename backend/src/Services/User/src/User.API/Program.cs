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
using Shared.Security;
using Shared.Swagger;
using User.API;
using User.Core.Consumers;
using User.Core.Database;
using User.Core.Services;

[assembly: MediatorOptions(Namespace = "User.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddMediator();

builder.Services.AddSingleton<UserContext>(o =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION") ??
                           builder.Configuration.GetConnectionString("MongoDB");

    var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
    var options = new InstrumentationOptions { CaptureCommandText = true };
    clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber(options));

    var client = new MongoClient(clientSettings);
    return new UserContext(client, database: "user");
});

builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(includeNamespace: true));

    config.AddConsumer<CreateUser>();
    config.AddConsumer<ReviewHost>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") ??
                               builder.Configuration.GetConnectionString("RabbitMQ");

        cfg.Host(new Uri(connectionString!));
        cfg.ConfigureEndpoints(context);
    });
});


builder.Services.AddHttpClient<ReservationsService>(service =>
{
    var url = Environment.GetEnvironmentVariable("RESERVATIONS_SERVICE_URL") ??
              builder.Configuration.GetConnectionString("ReservationsService");
    service.BaseAddress = new Uri(url!);
});

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger("User.API");

// Add FluentValidation to Swagger
builder.Services.AddValidatorsFromAssemblyContaining<UserContext>();
builder.Services.AddFluentValidationRulesToSwagger();

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("User.API"))
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
