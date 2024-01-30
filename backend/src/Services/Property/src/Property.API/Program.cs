using FluentValidation;
using MassTransit;
using Mediator;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
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
    var storagePath = Path.Combine(builder.Environment.WebRootPath, "property_images");
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

Endpoints.Map(app);

app.Run();
