namespace Property.Core.Model;

public sealed class Review
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid PropertyId { get; set; }
    public User Customer { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
