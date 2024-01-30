using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Model;

namespace Reservation.Core.Commands;

public static class DeleteReservation
{
    public record Command(Guid ReservationId, Guid UserId) : ICommand<Result>;

    public class ReservationNotFound() : Error("Reservation could not be found");

    public class Handler(ReservationContext context, ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var result = await context
                .Reservations
                .DeleteOneAsync(r => r.Id == command.ReservationId &&
                                     r.Status == ReservationStatus.Requested &&
                                     r.Customer.Id == command.UserId, cancellationToken);

            var success = result.IsAcknowledged && result.DeletedCount == 1;
            if (!success)
            {
                logger.LogWarning("Reservation {ReservationId} delete failed by user {UserId}",
                                   command.ReservationId, command.UserId);
                return new ReservationNotFound();
            }

            logger.LogInformation("Reservation {ReservationId} deleted by user {UserId}",
                                   command.ReservationId, command.UserId);
            return Result.Ok();
        }
    }
}
