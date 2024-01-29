using MassTransit;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Property.Core.Events;
using Property.Core.Model;

namespace Property.Core.Consumers;

public sealed class CreateUser(PropertyContext context, ILogger<CreateUser> logger) : IConsumer<UserRegistered>
{
    public async Task Consume(ConsumeContext<UserRegistered> consumer)
    {
        var @event = consumer.Message;

        var user = await context.Users.FindAsync(@event.UserId);
        if (user is not null)
        {
            logger.LogWarning("User with id {UserId} already exists, skipping creation", @event.UserId);
            return;
        }

        user = new User
        {
            Id = @event.UserId,
            Email = @event.Email,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            Role = @event.Role
        };

        context.Add(user);
        await context.SaveChangesAsync();

        logger.LogInformation("Created customer with email {Email}", user.Email);
    }
}