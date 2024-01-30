using FluentValidation;
using MassTransit;
using Mediator;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using MongoDB.Driver;
using Shared.Security;
using Shared.Swagger;
using User.API;
using User.Core.Consumers;
using User.Core.Database;

[assembly: MediatorOptions(Namespace = "User.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
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
    config.AddConsumer<ReviewHost>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") ??
                               builder.Configuration.GetConnectionString("RabbitMQ");

        cfg.Host(new Uri(connectionString!));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger("User.API");

// Add FluentValidation to Swagger
builder.Services.AddValidatorsFromAssemblyContaining<UserContext>();
builder.Services.AddFluentValidationRulesToSwagger();

var app = builder.Build();

app.UseCORS();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapEndpoints();

app.Run();
