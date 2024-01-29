using FluentValidation;
using FluentValidation.Results;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Property.Core.Database;

namespace Property.Core.Queries;

using Property.Core.Model;

public static class SearchProperties
{
    public record Query(string Location, int People, DateOnly Start, DateOnly End, string? Sort, int? Page = 0)
        : IQuery<IQueryable<Result>>
    {
        private static readonly Validator Validator = new();
        public ValidationResult Validate() => Validator.Validate(this);
    }

    public class Result
    {
        public Property? Property { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Location).NotEmpty().MinimumLength(2).MaximumLength(100);
            RuleFor(x => x.People).InclusiveBetween(1, 100);

            RuleFor(x => x.Start)
                .NotEmpty()
                .Must(d => d >= DateOnly.FromDateTime(DateTime.UtcNow))
                .Must((request, start) => start <= request.End)
                .WithMessage("Start date must be before end date");

            RuleFor(x => x.End)
                .NotEmpty()
                .Must((request, end) => end >= request.Start)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Sort)
                .Must(s => s is "CHEAPEST" or "MOST_EXPENSIVE")
                .When(q => q.Sort is not null)
                .WithMessage("Sort must be one of: CHEAPEST, MOST_EXPENSIVE");
        }
    }

    public class Handler(PropertyContext context, ILogger<Handler> logger) : IQueryHandler<Query, IQueryable<Result>>
    {
        private const int PageSize = 6;

        public ValueTask<IQueryable<Result>> Handle(Query query, CancellationToken cancellationToken)
        {
            var pageNumber = query.Page ?? 0;
            var nights = Math.Max(query.End.DayNumber - query.Start.DayNumber, 1);

            var properties = context.Slots
                .AsNoTrackingWithIdentityResolution()
                .Where(s => EF.Functions.ILike(s.Property.Location, $"%{query.Location}%"))
                .Where(s => query.People <= s.Property.MaxPeople)
                .Where(s => s.IsAvailable && query.Start >= s.Start && query.End <= s.End)
                .Where(x => !context.Slots.Any(s => !s.IsAvailable &&
                                                     s.End >= query.Start &&
                                                     s.Start <= query.End))
                .Select(s => new Result
                {
                    Property = s.Property,
                    TotalPrice = (s.CustomPrice ?? s.Property.UnitPrice) *
                                 (s.Property.PricingStrategy == PricingStrategy.PerNight ? nights : query.People)
                });

            var order = query.Sort switch
            {
                "MOST_EXPENSIVE" => properties.OrderBy(p => p.TotalPrice),
                "CHEAPEST" or _ => properties.OrderByDescending(p => p.TotalPrice)
            };

            var page = order
                .Skip(pageNumber * PageSize)
                .Take(PageSize);

            return ValueTask.FromResult(page);
        }
    }

}
