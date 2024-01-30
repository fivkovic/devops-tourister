using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("Gateway"));

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Gateway"))
    .WithTracing(config =>
    {
        // Listen to the YARP tracing activities
        config
            .AddSource("Yarp.ReverseProxy")
            .SetSampler(new AlwaysOnSampler())
            .AddOtlpExporter(c =>
            {
                var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ??
                               builder.Configuration.GetConnectionString("OTLPExporter");

                c.Endpoint = new Uri(endpoint!);
            });
    });

var app = builder.Build();

app.MapReverseProxy();
app.Run();