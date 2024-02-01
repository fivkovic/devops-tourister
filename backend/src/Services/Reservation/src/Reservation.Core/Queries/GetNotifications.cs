using Mediator;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Model;

namespace Reservation.Core.Queries
{
    public static class GetNotifications
    {
        public sealed record Query(Guid UserId) : IQuery<IQueryable<Notification>>;

        public sealed class Handler(ReservationContext context) : IQueryHandler<Query, IQueryable<Notification>>
        {
            public ValueTask<IQueryable<Notification>> Handle(Query query, CancellationToken cancellationToken)
            {
                var notifications = context.Notifications
                    .AsQueryable()
                    .Where(n => n.UserId == query.UserId);

                return new ValueTask<IQueryable<Notification>>(notifications);
            }
        }
    }
}
