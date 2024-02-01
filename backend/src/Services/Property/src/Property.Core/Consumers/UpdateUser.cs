using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Tourister.Events;

namespace Property.Core.Consumers;

public sealed class UpdateUser(PropertyContext context, ILogger<UpdateUser> logger) : IConsumer<UserUpdated>
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
            logger.LogWarning("Customer with id {UserId} does not exist, skipping update", @event.UserId);
            return;
        }

        logger.LogInformation("Updated customer with Id {UserId}", @event.UserId);
    }
}