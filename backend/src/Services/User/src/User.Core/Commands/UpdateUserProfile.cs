using FluentResults;
using FluentValidation;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tourister.Events;
using User.Core.Database;
using User.Core.Model;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace User.Core.Commands;

public static class UpdateUserProfile
{
    public sealed record Command(
        string FirstName,
        string LastName,
        string Residence) : ICommand<Result<UserProfile>>
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
            RuleFor(c => c.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(c => c.LastName).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Residence).NotEmpty().MaximumLength(100);
        }
    }

    public class UserNotFound() : Error("User could not be found");

    public class Handler(UserContext context, IPublishEndpoint publisher, ILogger<Handler> logger)
        : ICommandHandler<Command, Result<UserProfile>>
    {
        public async ValueTask<Result<UserProfile>> Handle(Command command, CancellationToken cancellationToken)
        {
            using var session = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);
            session.StartTransaction();

            var data = Builders<UserProfile>.Update
                .Set(u => u.FirstName, command.FirstName)
                .Set(u => u.LastName, command.LastName)
                .Set(u => u.Residence, command.Residence);

            var user = await context.UserProfiles.FindOneAndUpdateAsync(
                session,
                u => u.Id == command.UserId,
                data,
                cancellationToken: cancellationToken);

            if (user is null) return new UserNotFound();

            var @event = new UserUpdated(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Role,
                user.Residence
            );

            await publisher.Publish(@event, cancellationToken);
            await session.CommitTransactionAsync(cancellationToken);

            logger.LogInformation("User profile for user {UserId} updated", command.UserId);

            return user;
        }
    }
}