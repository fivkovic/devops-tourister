using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Property.Core.Model;

namespace Property.Core.Commands;

public static class CreateAvailability
{
    public record Command(DateOnly Start, DateOnly End, decimal? CustomPrice) : ICommand<Result<Slot>>
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
            RuleFor(x => x.PropertyId).NotEmpty();
            RuleFor(x => x.Start)
                .NotEmpty()
                .Must((request, start) => start <= request.End)
                .WithMessage("Start date must be before end date");
            RuleFor(x => x.End)
                .NotEmpty()
                .Must((request, end) => end >= request.Start)
                .WithMessage("End date must be after start date");
            RuleFor(x => x.CustomPrice)
                .InclusiveBetween(1, 100000)
                .When(x => x.CustomPrice.HasValue)
                .WithMessage("Price must be between 1 and 100000");
        }
    }

    public class PropertyNotFound() : Error("Property could not be found");

    public class Handler(PropertyContext context, ILogger<Handler> logger) : ICommandHandler<Command, Result<Slot>>
    {
        public async ValueTask<Result<Slot>> Handle(Command command, CancellationToken cancellationToken)
        {
            var property = await context
                .Properties
                .FirstOrDefaultAsync(p => p.Id == command.PropertyId &&
                                          p.OwnerId == command.UserId, cancellationToken);

            if (property is null) return new PropertyNotFound();

            var slot = new Slot(property, command.Start, command.End, isAvailable: true, command.CustomPrice);

            context.Add(slot);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Created availability period {SlotId} for property {PropertyId}",
                                   slot.Id, command.PropertyId);

            // Delete all available slots that are overlapping with the new slot (except the new slot itself)
            var deleted = await context
                .Slots
                .Where(s => s.Property.Id == command.PropertyId)
                .Where(s => s.Property.OwnerId == command.UserId)
                .Where(s => s.Id != slot.Id)
                .Where(s => s.IsAvailable)
                .Where(s => s.Start <= command.End && s.End >= command.Start)
                .ExecuteDeleteAsync(cancellationToken);

            logger.LogInformation("Deleted {Deleted} overlapping availability periods for property {PropertyId}",
                                   deleted, command.PropertyId);

            return slot;
        }
    }
}
