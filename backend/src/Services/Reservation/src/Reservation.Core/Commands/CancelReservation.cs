using FluentResults;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Events;
using Reservation.Core.Services;

namespace Reservation.Core.Commands;

using Reservation.Core.Model;

public static class CancelReservation
{
    public record Command(Guid ReservationId, Guid UserId) : ICommand<Result<Reservation>>;

    public class ReservationNotFound() : Error("Reservation could not be found");
    public class ReservationUncancellable() : Error("Reservation cannot be cancelled as the start date is too close");

    public class Handler(
        ReservationContext context,
        IPublishEndpoint eventBus,
        NotificationService notifications,
        ILogger<Handler> logger
    ) : ICommandHandler<Command, Result<Reservation>>
    {
        public async ValueTask<Result<Reservation>> Handle(Command command, CancellationToken cancellationToken)
        {
            var reservation = await context
                .Reservations
                .Find(r => r.Id == command.ReservationId &&
                           r.Customer.Id == command.UserId &&
                           r.Status == ReservationStatus.Accepted)
                .FirstOrDefaultAsync(cancellationToken);

            if (reservation is null) return new ReservationNotFound();

            var now = DateOnly.FromDateTime(DateTime.UtcNow);
            var cancelled = reservation.Cancel(now);

            if (!cancelled) return new ReservationUncancellable();

            using var session = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);
            session.StartTransaction();

            var result = await context
                .Reservations
                .ReplaceOneAsync(
                    session,
                    r => r.Id == reservation.Id,
                    reservation,
                    cancellationToken: cancellationToken
                );

            var success = result.IsAcknowledged && result.MatchedCount == 1;
            if (!success)
            {
                logger.LogWarning("Reservation {ReservationId} cancellation failed by user {UserId}",
                                   command.ReservationId, command.UserId);
                return new ReservationNotFound();
            }

            var @event = new ReservationCancelled(command.ReservationId);
            await eventBus.Publish(@event, cancellationToken);
            await session.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Reservation {ReservationId} cancelled by user {UserId}",
                                   command.ReservationId, command.UserId);

            await notifications.Send(@event, reservation, cancellationToken);

            return reservation;
        }
    }
}