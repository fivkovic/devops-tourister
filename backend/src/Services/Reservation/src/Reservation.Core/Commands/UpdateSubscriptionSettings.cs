using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Model;

namespace Reservation.Core.Commands;

public static class UpdateSubscriptionSettings
{
    public record Command(Subscription Subscription) : ICommand<Result>
    {
        private static readonly Validator Validator = new();

        public Guid UserId { get; private set; }
        public string UserRole { get; private set; }

        public void ByUser(Guid userId, string userRole)
        {
            UserId = userId;
            UserRole = userRole;
        }

        public ValidationResult Validate() => Validator.Validate(this);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Subscription)
                .IsInEnum()
                .Must(s => s.HasFlag(Subscription.None | Subscription.ReservationUpdated))
                .When(c => c.UserRole == "Customer")
                .WithMessage("Customers can only subscribe to reservation updates")
                .Must(s => !s.HasFlag(Subscription.ReservationUpdated))
                .When(c => c.UserRole == "Host")
                .WithMessage("Hosts can only subscribe to reviews and reservation events made by customers");
        }
    }

    public class UserNotFound() : Error("User could not be found");

    public class Handler(ReservationContext context, ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var data = Builders<User>
                .Update
                .Set(u => u.Subscription, command.Subscription);

            var result = await context.Users.UpdateOneAsync(
                u => u.Id == command.UserId,
                data,
                cancellationToken: cancellationToken
            );

            if (!result.IsAcknowledged || result.MatchedCount == 0) return new UserNotFound();

            logger.LogInformation("Updated subscription settings for user {UserId}", command.UserId);
            return Result.Ok();
        }
    }
}
