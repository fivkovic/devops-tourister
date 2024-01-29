using FluentResults;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Property.Core.Database;

namespace Property.Core.Queries;

using Property.Core.Model;

public static class GetProperty
{
    public record Query(Guid Id) : IRequest<Result<Property>>;

    public class PropertyNotFound() : Error("Property could not be found");

    public class Handler(PropertyContext context) : IRequestHandler<Query, Result<Property>>
    {
        public async ValueTask<Result<Property>> Handle(Query request, CancellationToken cancellationToken)
        {
            var property = await context
                .Properties
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Owner)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (property is null) return new PropertyNotFound();

            return property;
        }
    }
}
