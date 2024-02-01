namespace Reservation.Core.Model;

public enum ReservationStatus
{
    Requested,
    Accepted,
    Rejected,
    Cancelled
}

public sealed class Reservation
{
    public Guid Id { get; set; }
    public User Customer { get; set; }
    public Property Property { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public int People { get; set; }
    public decimal Price { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool ReviewedProperty { get; set; }
    public bool ReviewedHost { get; set; }
    public bool IsPending => Status == ReservationStatus.Requested;
    public bool IsAccepted => Status == ReservationStatus.Accepted;

    public bool Cancel(DateOnly now)
    {
        var cancellable = Status == ReservationStatus.Accepted && Start >= now.AddDays(1);
        if (!cancellable) return false;

        Status = ReservationStatus.Cancelled;
        Customer.ReservationsCancelled++;

        return true;
    }
}
