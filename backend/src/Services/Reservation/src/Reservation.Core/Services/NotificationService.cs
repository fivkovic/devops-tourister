using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Events;

namespace Reservation.Core.Services;

using Reservation.Core.Model;

public sealed class NotificationService(ReservationContext context, ILogger<NotificationService> logger)
{
    private static Notification Create<T>(T @event, Reservation reservation)
    {
        var (userId, subscription, title, message) = @event switch
        {
            ReservationRequested => (
                reservation.Property.OwnerId,
                Subscription.ReservationRequested,
                $"A new reservation for {reservation.Property.Name} has been requested",
                reservation.IsAccepted
                    ? "The reservation has been automatically accepted according to your preferences"
                    : "You can accept or reject the reservation from your dashboard"
            ),
            ReservationAccepted => (
                reservation.Customer.Id,
                Subscription.ReservationUpdated,
                $"Your reservation for {reservation.Property.Name} has been accepted",
                $"You can check in on {reservation.Start:ddd, dd MMM yyyy}"
            ),
            ReservationRejected => (
                reservation.Customer.Id,
                Subscription.ReservationUpdated,
                $"Your reservation for {reservation.Property.Name} has been rejected",
                "Contact the host of the property for more information"
            ),
            ReservationCancelled => (
                reservation.Property.OwnerId,
                Subscription.ReservationCancelled,
                $"The reservation {reservation.Id.ToString("N")[..16]} for your property {reservation.Property.Name} has been cancelled",
                "You can contact the customer for more information"
            ),
            PropertyReviewed review => (
                reservation.Property.OwnerId,
                Subscription.PropertyReviewed,
                $"{reservation.Customer.FullName} has reviewed {reservation.Property.Name} with a rating of {review.Rating} stars",
                review.Content
            ),
            HostReviewed review => (
                reservation.Property.OwnerId,
                Subscription.HostReviewed,
                $"{reservation.Customer.FullName} has reviewed you with a rating of {review.Rating} stars",
                review.Content
            ),
            _ => throw new ArgumentException("Invalid event type for a notification", typeof(T).FullName)
        };

        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Timestamp = DateTimeOffset.UtcNow,
            Title = title,
            Message = message,
            Subscription = subscription
        };
    }

    public async ValueTask Send<T>(T @event, Reservation reservation, CancellationToken cancellationToken = default)
    {
        var notification = Create(@event, reservation);
        var user = await context.Users
            .Find(u => u.Id == notification.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            logger.LogWarning("User {UserId} not found", notification.UserId);
            return;
        }

        if (!user.IsSubscribed(notification.Subscription)) return;

        await context.Notifications
            .InsertOneAsync(notification, cancellationToken: cancellationToken);

        logger.LogInformation("Notification {NotificationId} created for user {UserId}",
                               notification.Id, notification.UserId);
    }
}
