using FluentResults;
using Mediator;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using User.Core.Database;

namespace User.Core.Queries;

using User.Core.Model;

public class GetReviews
{
    public record Query(Guid UserId) : IQuery<Result<HostReviews>>;

    public record HostReviews(UserProfile Host, IQueryable<Review> Reviews);

    public class HostNotFound() : Error("Host not found");

    public class Handler(UserContext context) : IQueryHandler<Query, Result<HostReviews>>
    {
        public async ValueTask<Result<HostReviews>> Handle(Query query, CancellationToken cancellationToken)
        {
            var host = await context.UserProfiles
                .Find(u => u.Id == query.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (host is null) return new HostNotFound();

            var reviews = context.Reviews
                .AsQueryable()
                .Where(r => r.OwnerId == query.UserId);

            return new HostReviews(host, reviews);
        }
    }
}
