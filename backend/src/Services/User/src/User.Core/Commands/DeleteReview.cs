using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using User.Core.Database;
using User.Core.Model;

namespace User.Core.Commands;

public static class DeleteReview
{
    public record Command(Guid CustomerId, Guid ReviewId) : ICommand<Result>;

    public class ReviewNotFound() : Error("Review not found");

    public class Handler(UserContext context, ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var review = await context.Reviews.FindOneAndDeleteAsync(
                r => r.Id == command.ReviewId &&
                     r.Customer.Id == command.CustomerId,
                cancellationToken: cancellationToken
            );

            if (review is null) return new ReviewNotFound();
            logger.LogInformation("Deleted review {ReviewId} for host {OwnerId}", review.Id, review.OwnerId);

            var host = await context.UserProfiles
                .Find(u => u.Id == review.OwnerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (host is null)
            {
                logger.LogWarning("Host {OwnerId} not found, will not update their rating", review.OwnerId);
                return Result.Ok();
            }

            host.RemoveRating(review.Rating);

            var data = Builders<UserProfile>.Update
                .Set(u => u.AverageRating, host.AverageRating)
                .Set(u => u.NumberOfReviews, host.NumberOfReviews);

            var result = await context.UserProfiles.UpdateOneAsync(
                u => u.Id == host.Id,
                data,
                cancellationToken: cancellationToken
            );

            if (result.IsAcknowledged && result.MatchedCount == 1)
            {
                logger.LogInformation("Updated host {OwnerId} rating after deleting review", host.Id);
            }
            else
            {
                logger.LogWarning("Could not update host {OwnerId} rating after deleting review", host.Id);
            }

            return Result.Ok();
        }
    }
}
