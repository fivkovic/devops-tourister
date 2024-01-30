using Identity.Core.Events;
using Identity.Core.Model;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Core.Consumers;

public class DeleteUser(UserManager<User> userManager, ILogger<DeleteUser> logger) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> consumer)
    {
        var userId = consumer.Message.UserId.ToString();
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            logger.LogInformation("User with id {UserId} was not deleted because they don't exist", userId);
            return;
        }

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            logger.LogInformation("Deleted user with id {UserId}", userId);
            return;
        }

        var errors = string.Join(". ", result.Errors);
        logger.LogError("User with id {UserId} could not be deleted. Errors: {Errors}", userId, errors);
    }
}
