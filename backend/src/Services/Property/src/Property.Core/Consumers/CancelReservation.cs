using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Tourister.Events;

namespace Property.Core.Consumers;

public sealed class CancelReservation(PropertyContext context, ILogger<CancelReservation> logger)
    : IConsumer<ReservationCancelled>
{
    public async Task Consume(ConsumeContext<ReservationCancelled> @event)
    {
        var reservation = await context.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Slot)
            .Include(r => r.Property)
            .FirstOrDefaultAsync(r => r.Id == @event.Message.ReservationId);

        if (reservation is null) return;

        var slot = reservation.Slot;
        reservation.Cancel();

        await context.SaveChangesAsync();

        if (slot is null)
        {
            logger.LogWarning("Slot for reservation {ReservationId} not found", @event.Message.ReservationId);
            return;
        }

        var deleted = await context.Slots
            .Where(s => s.Id == slot.Id)
            .ExecuteDeleteAsync();

        if (deleted > 0)
        {
            logger.LogInformation("Slot {SlotId} deleted for reservation {ReservationId}", slot.Id, reservation.Id);
        }
    }
}
