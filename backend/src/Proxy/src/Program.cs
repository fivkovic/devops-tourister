using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("Gateway"));

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Proxy"))
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
    })
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

app.MapPrometheusScrapingEndpoint();

app.MapReverseProxy();

app.Run();