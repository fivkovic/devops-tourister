namespace Property.Core.Model;

public enum ReservationStatus
{
    Pending,
    Accepted,
    Rejected,
    Cancelled
}

public sealed class Reservation
{
    public Guid Id { get; set; }
    public User Customer { get; set; }
    public Property Property { get; set; } = null!;
    public Slot? Slot { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public int People { get; set; }
    public decimal Price { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Reservation() { }

    public Reservation(
        DateTimeOffset now,
        Property property,
        User customer,
        DateOnly start,
        DateOnly end,
        int people,
        decimal? customUnitPrice = null
    )
    {
        CreatedAt = now;
        Customer = customer;
        Property = property;
        Start = start;
        End = end;
        People = people;
        Status = property.AutoAcceptReservations ? ReservationStatus.Accepted : ReservationStatus.Pending;
        Price = property.CalculatePrice(Start, End, People, customUnitPrice);
    }

    public void Cancel()
    {
        if (Status != ReservationStatus.Accepted) return;
        Status = ReservationStatus.Cancelled;
        Slot = null;
    }

    public void Accept(Slot slot)
    {
        if (Status != ReservationStatus.Pending) return;
        Status = ReservationStatus.Accepted;
        Slot = slot;
    }

    public Review Review(string content, int rating, DateTimeOffset timestamp)
    {
        Property.Rate(rating);
        return new Review
        {
            ReservationId = Id,
            PropertyId = Property.Id,
            Customer = Customer,
            Content = content,
            Rating = rating,
            Timestamp = timestamp
        };
    }
}
