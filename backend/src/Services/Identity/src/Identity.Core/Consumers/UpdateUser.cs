using Identity.Core.Database;
using Identity.Core.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Core.Consumers;

public sealed class UpdateUser(IdentityContext context, ILogger<UpdateUser> logger) : IConsumer<UserUpdated>
{
    public async Task Consume(ConsumeContext<UserUpdated> consumer)
    {
        var @event = consumer.Message;
        var updated = await context.Users
            .Where(u => u.Id == @event.UserId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.FirstName, @event.FirstName)
                                      .SetProperty(u => u.LastName, @event.LastName));

        if (updated == 0)
        {
            logger.LogWarning("User with id {UserId} does not exist, skipping update", @event.UserId);
            return;
        }

        logger.LogInformation("Updated user with Id {UserId}", @event.UserId);
    }
}