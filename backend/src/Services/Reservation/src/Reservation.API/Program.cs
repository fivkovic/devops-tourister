using MassTransit;
using Mediator;
using MongoDB.Driver;
using Reservation.Core.Consumers;
using Reservation.Core.Database;

[assembly: MediatorOptions(Namespace = "Reservation.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediator();

builder.Services.AddSingleton(o =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION") ??
                           builder.Configuration.GetConnectionString("MongoDB");

    var client = new MongoClient(connectionString);
    return new ReservationContext(client, databaseName: "reservation");
});

builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(includeNamespace: true));

    config.AddConsumer<CreateReservation>();
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

app.Run();
