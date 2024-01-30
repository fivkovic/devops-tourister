namespace Reservation.Core.Model;

[Flags]
public enum Subscription
{
    None = 0,
    ReservationRequested = 1,
    ReservationCancelled = 2,
    ReservationUpdated = 4, // Accepted or Rejected
    PropertyReviewed = 8,
    HostReviewed = 16,
}

public sealed class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Message { get; set; }
    public Subscription Subscription { get; set; }
}
