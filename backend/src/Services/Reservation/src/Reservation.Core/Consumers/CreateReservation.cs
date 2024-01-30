using MassTransit;
using Microsoft.Extensions.Logging;
using Reservation.Core.Database;
using Reservation.Core.Events;

namespace Reservation.Core.Consumers;

public sealed class CreateReservation(
    ReservationContext context,
    ILogger<CreateReservation> logger) : IConsumer<ReservationRequested>
{
    public async Task Consume(ConsumeContext<ReservationRequested> consumer)
    {
        var reservation = consumer.Message.Reservation;
        await context.Reservations.InsertOneAsync(reservation);

        logger.LogInformation("Reservation {ReservationId} created by {UserId}",
                               reservation.Id, reservation.Customer.Id);

        if (reservation.IsAccepted)
        {
            var @event = new ReservationAccepted(reservation.Id);
            await consumer.Publish(@event);
            logger.LogInformation("Reservation {ReservationId} will be automatically accepted for customer {CustomerId}",
                                   reservation.Id, reservation.Customer.Id);
        }
    }
}
