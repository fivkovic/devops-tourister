using FluentResults;
using FluentValidation;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Property.Core.Events;
using Property.Core.Model;

using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Property.Core.Commands;

public static class RequestReservation
{
    public record Command(DateOnly Start, DateOnly End, int People) : ICommand<Result<Reservation>>
    {
        private static readonly Validator Validator = new();

        public Guid UserId { get; private set; }
        public Guid PropertyId { get; private set; }

        public void ByUser(Guid userId) => UserId = userId;
        public void WithPropertyId(Guid id) => PropertyId = id;

        public ValidationResult Validate() => Validator.Validate(this);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.People).InclusiveBetween(1, 100);

            RuleFor(x => x.Start)
                .NotEmpty()
                .Must(d => d >= DateOnly.FromDateTime(DateTime.UtcNow))
                .Must((request, start) => start <= request.End)
                .WithMessage("Start date must be before end date");

            RuleFor(x => x.End)
                .NotEmpty()
                .Must((request, end) => end >= request.Start)
                .WithMessage("End date must be after start date");
        }
    }

    public class UserNotFound() : Error("The user could not be found.");
    public class PropertyUnavailable() : Error("The property could not be found or is unavailable for the requested date.");
    public class TooManyPeople() : Error("There are too many people for this property.");

    public class Handler(PropertyContext context, IPublishEndpoint publisher, ILogger<Handler> logger)
        : ICommandHandler<Command, Result<Reservation>>
    {
        public async ValueTask<Result<Reservation>> Handle(Command command, CancellationToken cancellationToken)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            var customer = await context.Users
                .Where(c => c.Id == command.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (customer is null) return new UserNotFound();

            var slot = await context.Slots
                .Include(s => s.Property)
                .ThenInclude(p => p.Owner)
                .Where(s => s.Property.Id == command.PropertyId)
                .Where(s => s.IsAvailable && command.Start >= s.Start && command.End <= s.End)
                .Where(x => !context.Slots.Any(s => !s.IsAvailable &&
                                                     s.End >= command.Start &&
                                                     s.Start <= command.End))
                .FirstOrDefaultAsync(cancellationToken);

            if (slot is null) return new PropertyUnavailable();

            var property = slot.Property;
            if (command.People > property.MaxPeople) return new TooManyPeople();

            var reservation = new Reservation(
                DateTimeOffset.UtcNow,
                property,
                customer,
                command.Start,
                command.End,
                command.People,
                slot.CustomPrice
            );

            context.Add(reservation);

            var @event = new ReservationRequested(reservation);

            await context.SaveChangesAsync(cancellationToken);
            await publisher.Publish(@event, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Reservation {ReservationId} created by user {UserId}",
                                   reservation.Id, command.UserId);

            return reservation;
        }
    }

}
