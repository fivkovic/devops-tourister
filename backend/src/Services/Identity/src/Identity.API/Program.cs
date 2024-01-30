using FluentValidation;
using Identity.API;
using Identity.Core.Consumers;
using Identity.Core.Database;
using Identity.Core.Model;
using MassTransit;
using Mediator;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Security;
using Shared.Swagger;

[assembly: MediatorOptions(Namespace = "Identity.API", ServiceLifetime = ServiceLifetime.Scoped)]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddMediator();

builder.Services.AddDbContextPool<IdentityContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("PG_CONNECTION") ??
        builder.Configuration.GetConnectionString("Postgres");

    options.UseNpgsql(connectionString);
});

builder.Services.AddMassTransit(config =>
{
    config.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(includeNamespace: true));

    config.AddConsumer<UpdateUser>();
    config.AddConsumer<DeleteUser>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION") ??
                               builder.Configuration.GetConnectionString("RabbitMQ");

        cfg.Host(new Uri(connectionString!));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<IdentityContext>()
    .AddSignInManager<SignInManager<User>>()
    .AddDefaultTokenProviders();

// Add Authorization
builder.Services.AddAuthorization();
builder.Services.AddJwtAuthentication();

// Add Swagger
builder.Services.AddSwagger("Identity.API");

// Add FluentValidation to Swagger
builder.Services.AddValidatorsFromAssemblyContaining<IdentityContext>();
builder.Services.AddFluentValidationRulesToSwagger();

var app = builder.Build();

app.UseCORS();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
        await context.Database.MigrateAsync();
}

Endpoints.Map(app);

app.Run();
