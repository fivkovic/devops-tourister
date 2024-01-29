using MassTransit;
using Mediator;
using MongoDB.Driver;
using User.Core.Consumers;
using User.Core.Database;

[assembly: MediatorOptions(Namespace = "User.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediator();

builder.Services.AddSingleton(o =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION") ??
                           builder.Configuration.GetConnectionString("MongoDB");

    var client = new MongoClient(connectionString);

    return new UserContext(client, database: "user");
});

builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(includeNamespace: true));

    config.AddConsumer<CreateUser>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") ??
                               builder.Configuration.GetConnectionString("RabbitMQ");

        cfg.Host(new Uri(connectionString!));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
