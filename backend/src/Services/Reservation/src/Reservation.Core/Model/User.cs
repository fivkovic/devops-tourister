namespace Reservation.Core.Model;

public sealed class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string Role { get; set; }
    public int ReservationsCancelled { get; set; }
    public Subscription Subscription { get; set; }

    public bool IsSubscribed(Subscription subscription) => Subscription.HasFlag(subscription);
}
