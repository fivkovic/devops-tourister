using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Property.Core.Events;
using Property.Core.Model;

namespace Property.Core.Consumers
{
    public sealed class AcceptReservation(PropertyContext context, ILogger<AcceptReservation> logger)
        : IConsumer<ReservationAccepted>
    {
        public async Task Consume(ConsumeContext<ReservationAccepted> @event)
        {
            var reservation = await context.Reservations
                .Include(r => r.Slot)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(r => r.Id == @event.Message.ReservationId);

            if (reservation is null)
            {
                logger.LogWarning("Reservation {ReservationId} not found", @event.Message.ReservationId);
                return;
            }

            var slot = new Slot
            {
                IsAvailable = false,
                Property = reservation.Property,
                Start = reservation.Start,
                End = reservation.End
            };

            reservation.Accept(slot);
            reservation.Slot = slot; // Why doesn't it work without this? (the same call is in reservation.Accept)

            await context.SaveChangesAsync();

            logger.LogInformation("Slot {SlotId} created for reservation {ReservationId}",
                                   slot.Id, reservation.Id);
        }
    }
}