using FluentResults;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Events;

namespace Reservation.Core.Commands;

using Reservation.Core.Model;

public static class UpdateReservation
{
    public record Command(Guid ReservationId, Guid UserId, bool Accepted) : ICommand<Result<Reservation>>;

    public class ReservationNotFound() : Error("Reservation could not be found or has already been processed");

    public class Handler(ReservationContext context, IPublishEndpoint eventBus, ILogger<Handler> logger)
        : ICommandHandler<Command, Result<Reservation>>
    {
        public async ValueTask<Result<Reservation>> Handle(Command command, CancellationToken cancellationToken)
        {
            var status = command.Accepted ? ReservationStatus.Accepted : ReservationStatus.Rejected;
            var data = Builders<Reservation>.Update.Set(r => r.Status, status);

            using var session = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);
            session.StartTransaction();

            var reservation = await context.Reservations
                .FindOneAndUpdateAsync(
                    session,
                    r => r.Id == command.ReservationId &&
                         r.Property.OwnerId == command.UserId &&
                         r.Status == ReservationStatus.Requested,
                    data,
                    cancellationToken: cancellationToken
                );

            if (reservation is null) return new ReservationNotFound();

            if (command.Accepted)
            {
                var @event = new ReservationAccepted(reservation.Id);
                await eventBus.Publish(@event, cancellationToken);
                logger.LogInformation("Reservation {ReservationId} manually accepted by user {OwnerId}",
                                       reservation.Id, reservation.Property.OwnerId);
            }

            await session.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Reservation {ReservationId} updated by user {UserId}",
                                   command.ReservationId, command.UserId);

            return reservation;
        }
    }
}
