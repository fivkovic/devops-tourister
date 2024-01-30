using FluentResults;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;

namespace Property.Core.Commands;

public static class DeleteReview
{
    public record Command(Guid CustomerId, Guid ReviewId) : ICommand<Result>;

    public class ReviewNotFound() : Error("Review not found");

    public class Handler(PropertyContext context, ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async ValueTask<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var review = await context.Reviews.FirstOrDefaultAsync(
                r => r.Id == command.ReviewId &&
                     r.Customer.Id == command.CustomerId,
                cancellationToken
            );

            if (review is null) return new ReviewNotFound();

            var property = await context.Properties
                .FirstOrDefaultAsync(u => u.Id == review.PropertyId, cancellationToken);

            property?.RemoveRating(review.Rating);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Deleted review {ReviewId} for property {PropertyId}", review.Id, review.PropertyId);
            if (property is null)
            {
                logger.LogWarning("Property {PropertyId} not found, will not update its rating", review.PropertyId);
            }
            else
            {
                logger.LogInformation("Updated property {PropertyId} rating after deleting review", review.PropertyId);
            }

            return Result.Ok();
        }
    }
}
