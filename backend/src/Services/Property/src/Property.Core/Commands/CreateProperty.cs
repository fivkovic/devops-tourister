using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Microsoft.Extensions.Logging;
using Property.Core.Database;

namespace Property.Core.Commands;

using Property.Core.Model;

public static class CreateProperty
{
    public record Command(
        string Name,
        string Description,
        string Location,
        int MaxPeople,
        decimal UnitPrice,
        PricingStrategy PricingStrategy
    ) : ICommand<Property>
    {
        private static readonly Validator Validator = new();

        public Guid OwnerId { get; private set; }
        public string[] Images { get; private set; } = [];

        public void ByUser(Guid userId) => OwnerId = userId;
        public void WithImages(string[] images) => Images = images;

        public ValidationResult Validate() => Validator.Validate(this);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(2000);

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.MaxPeople).InclusiveBetween(1, 100);
            RuleFor(x => x.UnitPrice).InclusiveBetween(1, 100000);
            RuleFor(x => x.PricingStrategy).IsInEnum();
            RuleFor(x => x.OwnerId).NotEmpty();
        }
    }

    public class Handler(PropertyContext context, ILogger<Handler> logger) : ICommandHandler<Command, Property>
    {
        public async ValueTask<Property> Handle(Command command, CancellationToken cancellationToken)
        {
            var property = new Property
            {
                OwnerId = command.OwnerId,
                Name = command.Name,
                Description = command.Description,
                Location = command.Location,
                MaxPeople = command.MaxPeople,
                UnitPrice = command.UnitPrice,
                PricingStrategy = command.PricingStrategy,
                Images = command.Images
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Created property {PropertyId} by user {UserId}",
                                   property.Id, command.OwnerId);

            return property;
        }
    }
}
