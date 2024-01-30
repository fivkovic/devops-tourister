using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reservation.Core.Database;

namespace Reservation.Core.Queries;

using Reservation.Core.Model;

public static class GetReservations
{
    public record Query(int Page, string Sort, ReservationStatus? Status)
        : IQuery<IQueryable<Reservation>>
    {
        private static readonly Validator Validator = new();

        public Guid UserId { get; private set; }
        public string UserRole { get; private set; }

        public void ByUser(Guid userId, string role)
        {
            UserId = userId;
            UserRole = role;
        }

        public ValidationResult Validate() => Validator.Validate(this);
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Status).IsInEnum();
            RuleFor(x => x.Sort)
                .Must(s => s is "OLDEST" or "NEWEST")
                .WithMessage("Sort must be one of: OLDEST, NEWEST");
        }
    }

    public class Handler(ReservationContext context, ILogger<Handler> logger)
        : IQueryHandler<Query, IQueryable<Reservation>>
    {
        private const int PageSize = 12;

        public ValueTask<IQueryable<Reservation>> Handle(Query query, CancellationToken cancellationToken)
        {
            var queryable = context.Reservations.AsQueryable();

            var reservations = query.UserRole switch
            {
                "Host" => queryable.Where(r => r.Property.OwnerId == query.UserId),
                "Customer" or _ => queryable.Where(r => r.Customer.Id == query.UserId),
            };

            if (query.Status is not null)
            {
                reservations = reservations.Where(r => r.Status == query.Status);
            }

            var order = query.Sort switch
            {
                "OLDEST" => reservations.OrderBy(p => p.CreatedAt),
                "NEWEST" or _ => reservations.OrderByDescending(p => p.CreatedAt)
            };

            var page = order
                .Skip(query.Page * PageSize)
                .Take(PageSize);

            logger.LogInformation("Running query for user {UserId} with params {Page}, {Sort}, {Status}",
                                   query.UserId, query.Page, query.Sort, query.Status);

            return ValueTask.FromResult(page);
        }
    }

}
