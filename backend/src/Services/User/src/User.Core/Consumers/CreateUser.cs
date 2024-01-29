using MassTransit;
using Microsoft.Extensions.Logging;
using User.Core.Database;
using User.Core.Events;
using User.Core.Model;

namespace User.Core.Consumers;

public sealed class CreateUser(UserContext context, ILogger<CreateUser> logger) : IConsumer<UserRegistered>
{
    public Task Consume(ConsumeContext<UserRegistered> consumer)
    {
        var @event = consumer.Message;
        var userProfile = new UserProfile
        {
            Id = @event.UserId,
            Role = @event.Role,
            Email = @event.Email,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            Residence = @event.Residence
        };

        context.UserProfiles.InsertOneAsync(userProfile);
        logger.LogInformation("User profile for user with email {Email} created", userProfile.Email);

        return Task.CompletedTask;
    }
}
