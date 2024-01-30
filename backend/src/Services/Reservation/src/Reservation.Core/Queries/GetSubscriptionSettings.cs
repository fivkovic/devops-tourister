using FluentResults;
using Mediator;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Model;

namespace Reservation.Core.Queries;

public static class GetSubscriptionSettings
{
    public record Query(Guid UserId) : IQuery<Result<Subscription>>;

    public class UserNotFound() : Error("User not found");

    public class Handler(ReservationContext context) : IQueryHandler<Query, Result<Subscription>>
    {
        public async ValueTask<Result<Subscription>> Handle(Query query, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Find(u => u.Id == query.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null) return new UserNotFound();

            return user.Subscription;
        }
    }
}
