using Mediator;
using MongoDB.Driver;
using Reservation.Core.Database;
using Reservation.Core.Model;

namespace Reservation.Core.Queries;

public static class HasActiveReservations
{
    public record Query(Guid UserId, string Role) : IQuery<bool>;

    public class Handler(ReservationContext context) : IQueryHandler<Query, bool>
    {
        public async ValueTask<bool> Handle(Query query, CancellationToken cancellationToken)
        {
            var reservations = context.Reservations;
            var now = DateOnly.FromDateTime(DateTime.Now);

            var r = query.Role switch
            {
                "Host" => reservations.Find(r => r.Property.OwnerId == query.UserId &&
                                                          r.Status == ReservationStatus.Accepted &&
                                                          r.End > now),

                "Customer" or _ => reservations.Find(r => r.Customer.Id == query.UserId &&
                                                          r.Status == ReservationStatus.Accepted &&
                                                          r.End > now)
            };

            return await r.AnyAsync(cancellationToken);
        }
    }
}
