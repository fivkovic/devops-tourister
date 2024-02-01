namespace User.Core.Model;

public sealed class Review
{
    public Guid Id { get; set; }
    public UserProfile Customer { get; set; }
    public Guid OwnerId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}