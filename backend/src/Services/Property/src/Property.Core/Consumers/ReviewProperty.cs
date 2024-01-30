using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Property.Core.Events;

namespace Property.Core.Consumers;

public sealed class ReviewProperty(PropertyContext context, ILogger<ReviewProperty> logger) : IConsumer<PropertyReviewed>
{
    public async Task Consume(ConsumeContext<PropertyReviewed> consumer)
    {
        var @event = consumer.Message;
        var reservation = await context
            .Reservations
            .Include(r => r.Property)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == @event.ReservationId);

        if (reservation is null)
        {
            logger.LogWarning("Reservation {ReservationId} not found, could not create review", @event.ReservationId);
            return;
        }

        var review = reservation.Review(@event.Content, @event.Rating, @event.Timestamp);

        context.Add(review);
        await context.SaveChangesAsync();

        logger.LogInformation("Reservation {ReservationId} reviewed by customer {UserId}",
                               reservation.Id, reservation.Customer.Id);
    }
}
