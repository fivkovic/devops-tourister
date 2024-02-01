using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tourister.Events;
using User.Core.Database;
using User.Core.Model;

namespace User.Core.Consumers;

public sealed class ReviewHost(UserContext context, ILogger<ReviewHost> logger) : IConsumer<HostReviewed>
{
    public async Task Consume(ConsumeContext<HostReviewed> consumer)
    {
        var @event = consumer.Message;

        var customer = await context.UserProfiles
            .Find(u => u.Id == @event.CustomerId)
            .FirstOrDefaultAsync();

        if (customer is null)
        {
            logger.LogWarning("Customer {CustomerId} not found, could not create review", @event.CustomerId);
            return;
        }

        var host = await context.UserProfiles
            .Find(u => u.Id == @event.OwnerId)
            .FirstOrDefaultAsync();

        if (host is null)
        {
            logger.LogWarning("Host {OwnerId} not found, could not create review", @event.OwnerId);
            return;
        }

        var review = host.Review(customer, @event.Rating, @event.Content, @event.Timestamp);
        var data = Builders<UserProfile>.Update
            .Set(u => u.AverageRating, host.AverageRating)
            .Set(u => u.NumberOfReviews, host.NumberOfReviews);

        using var session = await context.Client.StartSessionAsync();
        session.StartTransaction();

        await Task.WhenAll(
            context.Reviews.InsertOneAsync(session, review),
            context.UserProfiles.UpdateOneAsync(session, u => u.Id == host.Id, data)
        );

        await session.CommitTransactionAsync();

        logger.LogInformation("Host {OwnerId} reviewed by customer {CustomerId}",
                               host.Id, customer.Id);
    }
}
