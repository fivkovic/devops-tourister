using Mediator;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using User.Core.Database;
using User.Core.Model;

namespace User.Core.Queries;

public sealed class GetReviews
{
    public record Query(Guid UserId) : IQuery<IQueryable<Review>>;

    public class Handler(UserContext context) : IQueryHandler<Query, IQueryable<Review>>
    {
        public ValueTask<IQueryable<Review>> Handle(Query query, CancellationToken cancellationToken)
        {
            var reviews = context.Reviews
                .AsQueryable()
                .Where(r => r.OwnerId == query.UserId);

            return new ValueTask<IQueryable<Review>>(reviews);
        }
    }
}
