using MassTransit;
using Microsoft.Extensions.Logging;
using Reservation.Core.Database;
using Reservation.Core.Events;
using Reservation.Core.Model;

namespace Reservation.Core.Consumers
{
    public class CreateUser(ReservationContext context, ILogger<CreateUser> logger) : IConsumer<UserRegistered>
    {
        public Task Consume(ConsumeContext<UserRegistered> consumer)
        {
            var @event = consumer.Message;
            var user = new User
            {
                Id = @event.UserId,
                Role = @event.Role,
                Email = @event.Email,
                FirstName = @event.FirstName,
                LastName = @event.LastName
            };

            context.Users.InsertOneAsync(user);
            logger.LogInformation("User with email {Email} created", user.Email);

            return Task.CompletedTask;
        }
    }
}
