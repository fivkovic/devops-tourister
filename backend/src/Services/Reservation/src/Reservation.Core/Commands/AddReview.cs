using FluentResults;
using FluentValidation;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Events;
using Reservation.Core.Services;

using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Reservation.Core.Commands;

using Reservation.Core.Model;

public static class AddReview
{
    public record Command(Guid Id, int Rating, string Content, string Type) : ICommand<Result>
    {
        private static readonly Validator Validator = new();

        public Guid UserId { get; private set; }

        public void ByUser(Guid userId) => UserId = userId;
        public ValidationResult Validate() => Validator.Validate(this);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.UserId).NotEmpty();
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.Rating).InclusiveBetween(1, 5);
            RuleFor(command => command.Content).NotEmpty().MaximumLength(500);
            RuleFor(command => command.Type).NotEmpty().Must(type => type is "PROPERTY" or "HOST");
        }
    }

    public class ReservationNotFound() : Error("Reservation not found or has already been reviewed");

    public class Handler(
        ReservationContext context,
        IPublishEndpoint eventBus,
        NotificationService notifications,
        ILogger<Handler> logger
    ) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var isProperty = command.Type == "PROPERTY";
            var data = Builders<Reservation>.Update.Set(
                r => isProperty ? r.ReviewedProperty : r.ReviewedHost,
                true
            );

            using var session = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);
            session.StartTransaction();

            var reservation = await context
                .Reservations
                .FindOneAndUpdateAsync(
                    session,
                    r => r.Id == command.Id &&
                         r.Customer.Id == command.UserId &&
                         r.Status == ReservationStatus.Accepted &&
                         isProperty ? !r.ReviewedProperty : !r.ReviewedHost,
                    data,
                    cancellationToken: cancellationToken
                );

            if (reservation == null) return new ReservationNotFound();

            var @event = GetEvent(command, reservation);
            await eventBus.Publish(@event, cancellationToken);
            await session.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("{Type} {Id} reviewed by {UserId}", command.Type, command.Id, command.UserId);

            await notifications.Send(@event, reservation, cancellationToken);

            return Result.Ok();
        }

        private static object GetEvent(Command command, Reservation reservation)
        {
            var isProperty = command.Type == "PROPERTY";
            return isProperty
            ? new PropertyReviewed(
                reservation.Id,
                command.Rating,
                command.Content,
                DateTimeOffset.UtcNow
            )
            : new HostReviewed(
                reservation.Customer.Id,
                reservation.Property.OwnerId,
                command.Rating,
                command.Content,
                DateTimeOffset.UtcNow
            );
        }
    }
}
