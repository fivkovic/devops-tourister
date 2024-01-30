using FluentResults;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using User.Core.Database;
using User.Core.Events;
using User.Core.Services;

namespace User.Core.Commands;

public static class DeleteUser
{
    public record Command(Guid UserId, string AccessToken) : ICommand<Result>;

    public class UserNotFound() : Error("User not found");

    public class HasActiveReservations() : Error("You cannot delete your account while you have active reservations");

    public class CouldNotDelete() : Error("Could not delete user, please try again later");

    public class Handler(
        ReservationsService reservations,
        UserContext context,
        IPublishEndpoint publisher,
        ILogger<Handler> logger
    ) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            using var session = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);
            session.StartTransaction();

            var user = await context.UserProfiles
                .Find(session, u => u.Id == command.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null) return new UserNotFound();

            var request = await reservations.HasActiveReservations(command.AccessToken);
            if (!request.IsSuccess)
            {
                logger.LogError("Could not delete user with id {UserId}", command.UserId);
                return new CouldNotDelete();
            }

            var hasActiveReservations = request.Value;
            if (hasActiveReservations) return new HasActiveReservations();

            var @event = new UserDeleted(command.UserId);
            await context.UserProfiles.DeleteOneAsync(
                session,
                u => u.Id == command.UserId,
                cancellationToken: cancellationToken
            );

            await publisher.Publish(@event, cancellationToken);
            await session.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("Deleted user with id {UserId}", command.UserId);
            return Result.Ok();
        }
    }
}
