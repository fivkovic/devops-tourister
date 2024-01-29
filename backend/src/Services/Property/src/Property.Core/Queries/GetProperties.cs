using Mediator;
using Microsoft.EntityFrameworkCore;
using Property.Core.Database;

namespace Property.Core.Queries;

using Property.Core.Model;

public static class GetProperties
{
    public record Query(Guid OwnerId) : IQuery<IQueryable<Property>>;

    public class Handler(PropertyContext context) : IQueryHandler<Query, IQueryable<Property>>
    {
        public ValueTask<IQueryable<Property>> Handle(Query query, CancellationToken cancellationToken)
        {
            var page = context.Properties
                .AsNoTracking()
                .Where(x => x.OwnerId == query.OwnerId);

            return ValueTask.FromResult(page);
        }
    }
}
