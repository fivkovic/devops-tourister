using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;
using Tourister.Events;

namespace Reservation.Core.Consumers;

using Reservation.Core.Model;
using Reservation.Core.Services;

public sealed class AcceptReservation(
    ReservationContext context,
    NotificationService notifications,
    ILogger<AcceptReservation> logger
) : IConsumer<ReservationAccepted>
{
    public async Task Consume(ConsumeContext<ReservationAccepted> consumer)
    {
        var @event = consumer.Message;
        var reservation = await context
            .Reservations
            .Find(r => r.Id == @event.ReservationId)
            .FirstOrDefaultAsync();

        if (reservation is null)
        {
            logger.LogWarning("Reservation {ReservationId} not found", @event.ReservationId);
            return;
        }

        logger.LogInformation("Reservation {ReservationId} accepted", @event.ReservationId);

        // Reject all other pending reservations for the same property that overlap with this one
        var data = Builders<Reservation>.Update.Set(r => r.Status, ReservationStatus.Rejected);
        var result = await context.Reservations.UpdateManyAsync(
            r => r.Id != reservation.Id &&
                 r.Status == ReservationStatus.Requested &&
                 r.Property.Id == reservation.Property.Id &&
                 r.Start <= reservation.End && r.End >= reservation.Start,
            data
        );

        logger.LogInformation("Rejected {Count} overlapping reservations for accepted reservation {ReservationId}",
                               result.ModifiedCount, reservation.Id);

        await notifications.Send(@event, reservation);
    }
}
