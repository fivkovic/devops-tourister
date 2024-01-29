using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Property.Core.Consumers;
using Property.Core.Database;

[assembly: MediatorOptions(Namespace = "Property.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

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

    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") ??
                               builder.Configuration.GetConnectionString("RabbitMQ");

        cfg.Host(new Uri(connectionString!));
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PropertyContext>();
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any()) await context.Database.MigrateAsync();
}

app.Run();
