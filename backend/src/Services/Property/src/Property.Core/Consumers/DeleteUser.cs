using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Property.Core.Events;

namespace Property.Core.Consumers;

public class DeleteUser(PropertyContext context, ILogger<DeleteUser> logger) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> consumer)
    {
        var userId = consumer.Message.UserId;
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            logger.LogWarning("User with id {UserId} not found, could not delete.", userId);
            return;
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        logger.LogInformation("Deleted user with id {UserId}", userId);

        if (user.Role != "Host") return;

        var propertiesDeleted = await context.Properties
            .Where(p => p.OwnerId == userId)
            .ExecuteDeleteAsync();

        logger.LogInformation("Deleted {PropertiesDeleted} properties owned by user {UserId}",
                               propertiesDeleted, userId);

    }
}
