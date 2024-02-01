using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;

namespace Reservation.Core.Commands;

public static class DeleteNotification
{
    public record Command(Guid Id, Guid UserId) : ICommand<Result>;

    public class NotificationNotFound() : Error("Notification not found");

    public class Handler(ReservationContext context, ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var result = await context.Notifications
                .DeleteOneAsync(
                    n => n.Id == command.Id && n.UserId == command.UserId,
                    cancellationToken: cancellationToken
                );

            if (!result.IsAcknowledged || result.DeletedCount == 0)
            {
                return new NotificationNotFound();
            }

            logger.LogInformation("Notification {Id} deleted for user {UserId}", command.Id, command.UserId);

            return Result.Ok();
        }
    }
}
