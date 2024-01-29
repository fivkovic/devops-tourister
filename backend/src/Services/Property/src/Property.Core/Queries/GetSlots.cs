using Mediator;
using Microsoft.EntityFrameworkCore;
using Property.Core.Database;
using Property.Core.Model;

namespace Property.Core.Queries;

public static class GetSlots
{
    public record Query(DateOnly Start, DateOnly End) : IQuery<IQueryable<Slot>>
    {
        public Guid PropertyId { get; private set; }
        public Guid UserId { get; private set; }

        public void ByUser(Guid userId) => UserId = userId;
        public void WithPropertyId(Guid id) => PropertyId = id;
    }

    public class Handler(PropertyContext context) : IQueryHandler<Query, IQueryable<Slot>>
    {
        public ValueTask<IQueryable<Slot>> Handle(Query query, CancellationToken cancellationToken)
        {
            var slots = context.Slots
                .AsNoTracking()
                .Where(s => s.IsAvailable)
                .Where(s => s.Property.Id == query.PropertyId)
                .Where(s => s.Start >= query.Start)
                .Where(s => s.End <= query.End);

            return ValueTask.FromResult(slots);
        }
    }
}
