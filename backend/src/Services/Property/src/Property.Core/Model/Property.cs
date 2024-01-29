namespace Property.Core.Model;

public enum PricingStrategy
{
    PerNight,
    PerPerson
}

public sealed class Property
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public User Owner { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public decimal UnitPrice { get; set; }
    public int MaxPeople { get; set; }
    public PricingStrategy PricingStrategy { get; set; }
    public bool AutoAcceptReservations { get; set; }
    public string[] Images { get; set; } = [];
    public List<Review> Reviews { get; set; } = [];
    public int NumberOfReviews { get; set; }
    public double AverageRating { get; set; }
    public string Description { get; set; }

    public decimal CalculatePrice(DateOnly start, DateOnly end, int people, decimal? customUnitPrice)
    {
        var nights = Math.Max(end.DayNumber - start.DayNumber, 1);
        var price = (customUnitPrice ?? UnitPrice) *
                    (PricingStrategy == PricingStrategy.PerNight ? nights : people);

        return price;
    }

    public void Rate(int rating)
    {
        AverageRating = (NumberOfReviews * AverageRating + rating) / (NumberOfReviews + 1);
        NumberOfReviews++;
    }

    public void RemoveRating(int rating)
    {
        AverageRating = (NumberOfReviews * AverageRating - rating) / (NumberOfReviews - 1);
        NumberOfReviews--;
    }
}
