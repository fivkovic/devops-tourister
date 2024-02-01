using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;
using Property.Core.Model;

namespace Property.Core.Commands;

public static class UpdateSettings
{
    public record Command(
        Guid Id,
        bool AutoAccept,
        PricingStrategy PricingStrategy,
        decimal UnitPrice
    ) : ICommand<Result>
    {
        private static readonly Validator Validator = new();
        public Guid UserId { get; private set; }

        public void ByUser(Guid userId) => UserId = userId;
        public ValidationResult Validate() => Validator.Validate(this);
    }

    private class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.AutoAccept).NotNull();
            RuleFor(x => x.PricingStrategy).IsInEnum();
            RuleFor(x => x.UnitPrice).InclusiveBetween(1, 1000000);
        }
    }

    public class PropertyNotFound() : Error("Property not found");

    public class Handler(PropertyContext context, ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var updated = await context.Properties
                .Where(p => p.Id == command.Id)
                .Where(p => p.OwnerId == command.UserId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.AutoAcceptReservations, command.AutoAccept)
                                          .SetProperty(p => p.PricingStrategy, command.PricingStrategy)
                                          .SetProperty(p => p.UnitPrice, command.UnitPrice),
                    cancellationToken
                );

            if (updated == 0) return new PropertyNotFound();

            logger.LogInformation("Updated settings for property {PropertyId}", command.Id);
            return Result.Ok();
        }
    }
}
